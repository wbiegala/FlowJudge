using FlowJudge.API.Contracts;
using FlowJudge.API.Service.Controllers.Mappers;
using FlowJudge.API.Service.ErrorHandling;
using FlowJudge.API.Service.Extensions;
using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Http.Extensions;
using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Queries;
using Microsoft.AspNetCore.Mvc;

namespace FlowJudge.API.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RepositoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetRepositoriesAsync(
            [FromQuery] PaginationQueryParams paginationParams,
            CancellationToken cancellationToken = default)
        {
            var workspaceId = this.HttpContext.GetWorkspaceId();
            if (!workspaceId.HasValue)
                return ApplicationErrorMapper.ErrorResponse(
                    ErrorCodeGenerator.NotAcceptable("repository"),
                    "Workspace context is missing",
                    System.Net.HttpStatusCode.BadRequest);
            var userContext = this.HttpContext.User.GetUserContext();

            var query = new GetRepositoriesQuery(workspaceId.Value, userContext.Id, paginationParams.ToModel());
            var result = await _mediator.SendQueryAsync<GetRepositoriesQuery, PagedList<RepositoryListItem>>(query, cancellationToken);

            if (!result.IsSuccess)
                return result.Error!.ToResponse();

            var uniqueIntegrationsCount = result.Data!.Select(repo => repo.IntegrationId).Distinct().Count();
            var workspaceIntegrationsQuery = new GetIntegrationsQuery(
                workspaceId.Value,
                userContext.Id,
                new PageQuery { PageNumber = 1, PageSize = uniqueIntegrationsCount });

            var integrationsResult = await _mediator.SendQueryAsync<GetIntegrationsQuery, PagedList<IntegrationListItem>>(workspaceIntegrationsQuery, cancellationToken);

            if (!integrationsResult.IsSuccess)
                return result.Error!.ToResponse();

            return Ok(result.Data!.ToPagedResult(repo => repo.ToListItem(id => integrationsResult.Data!.First(i => i.Id == id))));
        }
    }
}
