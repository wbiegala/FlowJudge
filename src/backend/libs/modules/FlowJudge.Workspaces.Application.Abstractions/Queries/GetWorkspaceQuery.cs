using FlowJudge.Common.Application.Mediator;
using FlowJudge.Workspaces.Application.Abstractions.Models;

namespace FlowJudge.Workspaces.Application.Abstractions.Queries
{
    public sealed record GetWorkspaceQuery(Guid WorkspaceId, Guid UserId) : IQuery<WorkspaceData>;
}
