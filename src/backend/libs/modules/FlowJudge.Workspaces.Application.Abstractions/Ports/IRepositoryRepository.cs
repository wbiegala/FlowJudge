using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Repository.Model;

namespace FlowJudge.Workspaces.Application.Abstractions.Ports
{
    public interface IRepositoryRepository
    {
        Task<IEnumerable<RepositoryRoot>> GetRepositoriesByIntegrationAsync(IntegrationId integrationId, CancellationToken ct = default);
        Task AddRepositoryAsync(RepositoryRoot repository, CancellationToken ct = default);
        Task UpdateRepositoryAsync(RepositoryRoot repository, CancellationToken ct = default);
    }
}
