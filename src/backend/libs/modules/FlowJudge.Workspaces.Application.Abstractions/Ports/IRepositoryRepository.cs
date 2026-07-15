using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Repository.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Application.Abstractions.Ports
{
    public interface IRepositoryRepository
    {
        Task<IEnumerable<RepositoryRoot>> GetRepositoriesByIntegrationAsync(IntegrationId integrationId, CancellationToken ct = default);
        Task AddRepositoryAsync(RepositoryRoot repository, CancellationToken ct = default);
        Task UpdateRepositoryAsync(RepositoryRoot repository, CancellationToken ct = default);
        Task<PagedList<RepositoryListItem>> GetRepositoriesAsync(WorkspaceId workspace, PageQuery pagination, CancellationToken ct = default);
    }
}
