using FlowJudge.API.Contracts;
using FlowJudge.API.Contracts.Integrations;
using FlowJudge.API.Service.Controllers.Mappers;
using FlowJudge.API.Service.ErrorHandling;
using FlowJudge.API.Service.Extensions;
using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Http.Extensions;
using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Users.Application.Abstractions.Queries;
using FlowJudge.Users.Application.Models;
using FlowJudge.Workspaces.Application.Abstractions.Commands;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Queries;
using FlowJudge.Workspaces.Domain.Integration.Model;
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

        public IntegrationsController(IMediator mediator)
        {
            _mediator = mediator;
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

        [HttpPost("{provider}")]
        public async Task<IActionResult> CreateIntegrationAsync(
            [FromRoute] string provider,
            [FromBody] CreateIntegrationRequest request,
            CancellationToken cancellationToken = default)
        {
            var isProviderValid = Enum.TryParse<IntegrationProvider>(provider, true, out var integrationProvider);
            if (!isProviderValid)
                return ApplicationErrorMapper.ErrorResponse(
                    ErrorCodeGenerator.NotAcceptable("integration"),
                    "Invalid integration provider",
                    System.Net.HttpStatusCode.BadRequest);

            var workspaceId = this.HttpContext.GetWorkspaceId();
            if (!workspaceId.HasValue)
                return ApplicationErrorMapper.ErrorResponse(
                    ErrorCodeGenerator.NotAcceptable("integration"),
                    "Workspace context is missing",
                    System.Net.HttpStatusCode.BadRequest);

            //TODO: validate request

            var userContext = this.HttpContext.User.GetUserContext();
            var command = new CreateIntegrationCommand(request.Name, integrationProvider, workspaceId.Value, userContext.Id);
            var result = await _mediator.SendCommandAsync<CreateIntegrationCommand, Guid>(command, cancellationToken);

            if (!result.IsSuccess)
                return result.Error!.ToResponse();

            return Ok(new CreatedResponse(result.Data));
        }
    }
}
