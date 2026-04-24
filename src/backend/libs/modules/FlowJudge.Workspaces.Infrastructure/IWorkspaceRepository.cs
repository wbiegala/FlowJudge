using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Infrastructure
{
    public interface IWorkspaceRepository
    {
        Task<WorkspaceRoot?> GetWorkspaceByAggregateIdAsync(WorkspaceId workspaceId, CancellationToken ct = default);
        Task AddWorkspaceAsync(WorkspaceRoot workspace, CancellationToken ct = default);
        Task UpdateWorkspaceAsync(WorkspaceRoot workspace, CancellationToken ct = default);
    }
}
