using FlowJudge.Common.Application.Mediator;
using FlowJudge.Workspaces.Domain.Workspace.ReadModels;

namespace FlowJudge.Workspaces.Application.Queries
{
    public sealed record GetWorkspaceQuery(Guid WorkspaceId, Guid UserId) : IQuery<WorkspaceData>;
}
