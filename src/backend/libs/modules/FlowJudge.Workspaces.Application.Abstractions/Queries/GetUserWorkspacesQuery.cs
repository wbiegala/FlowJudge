using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Workspaces.Application.Abstractions.Models;

namespace FlowJudge.Workspaces.Application.Abstractions.Queries
{
    public sealed record GetUserWorkspacesQuery(Guid UserId, PageQuery Pagination)
        : IQuery<PagedList<WorkspaceListItem>>;
}
