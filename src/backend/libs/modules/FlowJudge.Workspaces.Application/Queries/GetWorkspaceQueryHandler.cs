using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Workspaces.Application.Abstractions;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Abstractions.Queries;
using FlowJudge.Workspaces.Application.Extensions;
using FlowJudge.Workspaces.Domain.Workspace.Extensions;
using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Application.Queries
{
    internal sealed class GetWorkspaceQueryHandler : IQueryHandler<GetWorkspaceQuery, WorkspaceData>
    {
        private readonly IWorkspaceRepository _workspaceRepository;

        public GetWorkspaceQueryHandler(IWorkspaceRepository workspaceRepository)
        {
            _workspaceRepository = workspaceRepository;
        }

        public async Task<IResult<WorkspaceData>> HandleAsync(GetWorkspaceQuery query, CancellationToken cancellationToken = default)
        {
            var workspaceId = WorkspaceId.Create(query.WorkspaceId);
            var workspace = await _workspaceRepository.GetWorkspaceByAggregateIdAsync(workspaceId, cancellationToken);

            if (workspace is null || !workspace.IsUserMember(query.UserId))
            { 
                return ApplicationResultFactory.Failure<WorkspaceData>("Workspace not found.", ErrorCodes.WorkspaceNotFound);
            }

            return ApplicationResultFactory.Success(workspace.ToAbstraction());
        }
    }
}
