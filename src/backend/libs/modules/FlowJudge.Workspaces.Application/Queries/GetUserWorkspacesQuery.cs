using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Workspaces.Domain.Workspace.ReadModels;

namespace FlowJudge.Workspaces.Application.Queries
{
    public sealed record GetUserWorkspacesQuery(Guid UserId, PageQuery Pagination)
        : IQuery<PagedList<WorkspaceListItem>>;
}
