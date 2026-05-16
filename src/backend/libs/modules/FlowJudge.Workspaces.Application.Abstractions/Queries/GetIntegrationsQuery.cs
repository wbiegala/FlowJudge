using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Workspaces.Application.Abstractions.Models;

namespace FlowJudge.Workspaces.Application.Abstractions.Queries
{
    public sealed record GetIntegrationsQuery(Guid WorkspaceId, Guid IssuerId, PageQuery Pagination)
        : IQuery<PagedList<IntegrationListItem>>;
}
