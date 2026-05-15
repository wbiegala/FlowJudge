using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Application.Abstractions.Ports
{
    public interface IWorkspaceRepository
    {
        Task<WorkspaceRoot?> GetWorkspaceByAggregateIdAsync(WorkspaceId workspaceId, CancellationToken ct = default);
        Task AddWorkspaceAsync(WorkspaceRoot workspace, CancellationToken ct = default);
        Task UpdateWorkspaceAsync(WorkspaceRoot workspace, CancellationToken ct = default);
        Task<PagedList<WorkspaceListItem>> GetUserWorkspacesAsync(Guid userId, PageQuery pagination, CancellationToken ct = default);
        Task<WorkspaceRole?> GetUserRoleInWorkspaceAsync(WorkspaceId workspaceId, Guid userId, CancellationToken ct = default);
    }
}
