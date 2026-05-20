using FlowJudge.API.Contracts;
using FlowJudge.API.Contracts.Integrations.GitHub;
using FlowJudge.API.Service.Controllers.Mappers;
using FlowJudge.API.Service.Controllers.Redirects;
using FlowJudge.API.Service.ErrorHandling;
using FlowJudge.API.Service.Extensions;
using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Http.Extensions;
using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Users.Application.Abstractions.Queries;
using FlowJudge.Users.Application.Models;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Queries;
using FlowJudge.Workspaces.Application.Abstractions.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowJudge.API.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class IntegrationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IGitHubInstallationService _githubInstallationService;

        public IntegrationsController(
            IMediator mediator,
            IGitHubInstallationService githubInstallationService)
        {
            _mediator = mediator;
            _githubInstallationService = githubInstallationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetIntegrationsAsync(
            [FromQuery] PaginationQueryParams paginationParams,
            CancellationToken cancellationToken = default)
        {
            var workspaceId = this.HttpContext.GetWorkspaceId();
            if (!workspaceId.HasValue)
                return ApplicationErrorMapper.ErrorResponse(
                    ErrorCodeGenerator.NotAcceptable("integration"),
                    "Workspace context is missing",
                    System.Net.HttpStatusCode.BadRequest);
            var userContext = this.HttpContext.User.GetUserContext();

            var query = new GetIntegrationsQuery(workspaceId.Value, userContext.Id, paginationParams.ToModel());
            var result = await _mediator.SendQueryAsync<GetIntegrationsQuery, PagedList<IntegrationListItem>>(query, cancellationToken);

            if (!result.IsSuccess)
                return result.Error!.ToResponse();

            var creatorsIds = result.Data!.Select(item => item.CreatedBy).Distinct().ToList();
            var creatorDataTasks = creatorsIds.Select(id => _mediator.SendQueryAsync<GetUserDataQuery, UserData>(new GetUserDataQuery(id), cancellationToken));
            var creatorDataResults = await Task.WhenAll(creatorDataTasks);
            var creatorData = creatorDataResults
                .Where(r => r.IsSuccess)
                .Select(r => r.Data)
                .ToList();
            var getCreatorDataFunc = (Guid id) => creatorData.FirstOrDefault(d => d.UserId == id);

            return Ok(result.Data!.ToPagedResult(item => item.ToResponseItem(getCreatorDataFunc)));
        }

        #region GITHUB

        [HttpPost("github/install")]
        public async Task<IActionResult> ConnectIntegrationAsync(
            CancellationToken cancellationToken = default)
        {
            var workspaceId = this.HttpContext.GetWorkspaceId();
            if (!workspaceId.HasValue)
                return ApplicationErrorMapper.ErrorResponse(
                    ErrorCodeGenerator.NotAcceptable("integration"),
                    "Workspace context is missing",
                    System.Net.HttpStatusCode.BadRequest);

            var userContext = this.HttpContext.User.GetUserContext();

            var result = await _githubInstallationService.StartGitHubInstallationAsync(workspaceId.Value, userContext.Id, cancellationToken);

            if (!result.IsSuccess)
                return result.Error!.ToResponse();

            return Ok(new InstallGitHubIntegrationResponse { RedirectUrl = result.Data.RedirectUrl });
        }

        [HttpGet("github/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> GitHubSetupCallbackAsync(
            [FromQuery] GitHubSetupCallbackQueryParams callbackData,
            [FromServices] GitHubIntegrationRedirectionService redirectGithubService,
            [FromServices] ErrorPageRedirectionService redirectErrorService,
            CancellationToken cancellationToken = default)
        {
            var stateId = Guid.Parse(callbackData.state);
            var result = await _githubInstallationService.ConfirmGitHubInstallationAsync(
                stateId,
                callbackData.installation_id,
                callbackData.setup_action,
                cancellationToken);

            if (!result.IsSuccess)
            {
                var errorPageRedirectUrl = redirectErrorService.GetErrorPageReditectUrl(result.Error!);
                return Redirect(errorPageRedirectUrl);
            }

            var redirectUrl = redirectGithubService.GetGitHubInstallationCallbackSuccessRedirectUrl(
                result.Data!.WorkspaceId, result.Data!.InstallationStateId);

            return Redirect(redirectUrl);
        }

        [HttpGet("github/{installationStateId:guid}/repositories")]
        public async Task<IActionResult> GetGitHubInstallationRepositoriesAsync(
            [FromRoute] Guid installationStateId,
            CancellationToken cancellationToken = default)
        {
            var workspaceId = this.HttpContext.GetWorkspaceId();
            if (!workspaceId.HasValue)
                return ApplicationErrorMapper.ErrorResponse(
                    ErrorCodeGenerator.NotAcceptable("integration"),
                    "Workspace context is missing",
                    System.Net.HttpStatusCode.BadRequest);

            var result = await _githubInstallationService.GetRepositoriesForInstallationAsync(installationStateId, cancellationToken);
            if (!result.IsSuccess)
                return result.Error!.ToResponse();

            var response = result.Data!.Select(repo => new GetGitHubInstallationRepositoriesResponseItem
            {
                Id = repo.Id,
                Name = repo.Name,
                FullName = repo.FullName,
            }).ToArray();

            return Ok(response);
        }

        [HttpPost("github/{installationStateId:guid}/commit")]
        public async Task<IActionResult> CommitGitHubInstallationAsync(
            [FromRoute] Guid installationStateId,
            [FromBody] CommitGitHubIntegrationInstallationRequest request,
            CancellationToken cancellationToken = default)
        {
            var workspaceId = this.HttpContext.GetWorkspaceId();
            if (!workspaceId.HasValue)
                return ApplicationErrorMapper.ErrorResponse(
                    ErrorCodeGenerator.NotAcceptable("integration"),
                    "Workspace context is missing",
                    System.Net.HttpStatusCode.BadRequest);

            var userContext = this.HttpContext.User.GetUserContext();

            var result = await _githubInstallationService.CommitGitHubInstallationAsync(
                installationStateId,
                request.Name,
                request.RepositoriesConfiguration.Select(r => new GitHubInstallationRepositoryConfiguration
                {
                    GithubId = r.GithubId,
                    EnableTracking = r.Track
                }),
                userContext.Id,
                cancellationToken);

            if (!result.IsSuccess)
                return result.Error!.ToResponse();

            return Ok(new CreatedResponse(result.Data!));
        }

        #endregion
    }
}