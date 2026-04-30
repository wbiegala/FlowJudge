using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Workspaces.Domain.Workspace.Extensions;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.ReadModels;
using FlowJudge.Workspaces.Infrastructure;

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

            return ApplicationResultFactory.Success(workspace.ToReadModel());
        }
    }
}
