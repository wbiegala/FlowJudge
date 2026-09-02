namespace FlowJudge.Reviews.Application.Abstractions.Ports
{
    public interface IDomainSynchronizer
    {
        Task SynchronizeWorkspaceAsync(CancellationToken ct = default);
        Task SynchronizeIntegrationAsync(CancellationToken ct = default);
        Task SynchronizeRepositoryAsync(CancellationToken ct = default);
    }
}
