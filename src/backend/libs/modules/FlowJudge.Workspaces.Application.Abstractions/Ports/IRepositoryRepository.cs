using FlowJudge.Workspaces.Domain.Repository.Model;

namespace FlowJudge.Workspaces.Application.Abstractions.Ports
{
    public interface IRepositoryRepository
    {
        Task AddRepositoryAsync(RepositoryRoot repository, CancellationToken ct = default);
    }
}
