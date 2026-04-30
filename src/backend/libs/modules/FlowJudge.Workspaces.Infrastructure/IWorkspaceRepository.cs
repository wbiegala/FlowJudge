using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.ReadModels;

namespace FlowJudge.Workspaces.Infrastructure
{
    public interface IWorkspaceRepository
    {
        Task<WorkspaceRoot?> GetWorkspaceByAggregateIdAsync(WorkspaceId workspaceId, CancellationToken ct = default);
        Task AddWorkspaceAsync(WorkspaceRoot workspace, CancellationToken ct = default);
        Task UpdateWorkspaceAsync(WorkspaceRoot workspace, CancellationToken ct = default);

        Task<PagedList<WorkspaceListItem>> GetUserWorkspacesAsync(Guid userId, PageQuery pagination, CancellationToken ct = default);
    }
}
