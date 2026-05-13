using FlowJudge.API.Contracts;
using FlowJudge.API.Contracts.Workspaces;
using FlowJudge.API.Service.Controllers.Mappers;
using FlowJudge.API.Service.ErrorHandling;
using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Http.Extensions;
using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Users.Application.Abstractions.Queries;
using FlowJudge.Users.Application.Models;
using FlowJudge.Workspaces.Application.Abstractions.Commands;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlowJudge.API.Service.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class WorkspacesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkspacesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetWorkspacesAsync(
            [FromQuery] PaginationQueryParams paginationParams,
            CancellationToken ct = default)
        {
            var userContext = this.HttpContext.User.GetUserContext();
            var query = new GetUserWorkspacesQuery(userContext.Id, paginationParams.ToModel());

            var result = await _mediator.SendQueryAsync<GetUserWorkspacesQuery, PagedList<WorkspaceListItem>>(query, ct);
            if (!result.IsSuccess)
            {
                return result.Error!.ToResponse(System.Net.HttpStatusCode.BadRequest);
            }

            var ownerIds = result.Data!.Select(item => item.OwnerId).Distinct().ToList();
            var ownerDataTasks = ownerIds.Select(id => _mediator.SendQueryAsync<GetUserDataQuery, UserData>(new GetUserDataQuery(id), ct));
            var ownerDataResults = await Task.WhenAll(ownerDataTasks);
            var ownerData = ownerDataResults
                .Where(r => r.IsSuccess)
                .Select(r => r.Data)
                .ToList();
            var getOwnerDataFunc = (Guid id) => ownerData.FirstOrDefault(d => d.UserId == id);

            return Ok(result.Data!.ToPagedResult(item => item.ToResponseItem(getOwnerDataFunc)));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetWorkspaceAsync(
            [FromRoute] Guid id,
            CancellationToken ct = default)
        {
            var userContext = this.HttpContext.User.GetUserContext();
            var query = new GetWorkspaceQuery(id, userContext.Id);

            var result = await _mediator.SendQueryAsync<GetWorkspaceQuery, WorkspaceData>(query, ct);
            if (!result.IsSuccess)
            {
                var errorCode = result.Error!.IsNotFound()
                    ? System.Net.HttpStatusCode.NotFound
                    : System.Net.HttpStatusCode.BadRequest;
                return result.Error!.ToResponse(errorCode);
            }
            var allRelatedUserIds = result.Data!.GetRelatedUsersIds();
            var allRelatedUserTasks = allRelatedUserIds.Select(id => _mediator.SendQueryAsync<GetUserDataQuery, UserData>(new GetUserDataQuery(id), ct));
            var allRelatedUserResults = await Task.WhenAll(allRelatedUserTasks);
            var usersData = allRelatedUserResults
                .Where(r => r.IsSuccess)
                .Select(r => r.Data)
                .ToList();
            var getUserData = (Guid id) => usersData.FirstOrDefault(d => d!.UserId == id);

            return Ok(result.Data!.ToResponse(getUserData));
        }

        [HttpPost]
        public async Task<IActionResult> CreateWorkspaceAsync(
            [FromBody] CreateWorkspaceRequest request,
            CancellationToken ct = default)
        {
            //TODO: validation

            var userContext = this.HttpContext.User.GetUserContext();
            var command = new CreateWorkspaceCommand(request.Name, userContext.Id);

            var result = await _mediator.SendCommandAsync<CreateWorkspaceCommand, Guid>(command, ct);
            if (!result.IsSuccess)
            {
                return result.Error!.ToResponse(System.Net.HttpStatusCode.BadRequest);
            }

            return Ok(new CreatedResponse(result.Data));
        }

        [HttpPut("{id:Guid}")]
        public async Task<IActionResult> UpdateWorkspaceAsync(
            [FromRoute] Guid id,
            [FromBody] UpdateWorkspaceRequest request,
            CancellationToken ct = default)
        {
            //TODO: validation

            var userContext = this.HttpContext.User.GetUserContext();
            var command = new UpdateWorkspaceCommand
            {
                WorkspaceId = id,
                Name = request.Name,
                IssuerId = userContext.Id
            };

            var result = await _mediator.SendCommandAsync<UpdateWorkspaceCommand>(command, ct);
            if (!result.IsSuccess)
            {
                if (result.Error!.IsInsufficientPermissions())
                    return result.Error!.ToResponse(System.Net.HttpStatusCode.Forbidden);

                return result.Error!.ToResponse(System.Net.HttpStatusCode.BadRequest);
            }

            return NoContent();
        }
    }
}
