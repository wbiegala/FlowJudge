using FlowJudge.GitHub.Client.Contract;

namespace FlowJudge.GitHub.Client.Clients
{
    public interface IRepositoryClient
    {
        Task<GetInstallationRepositoriesResponse> GetInstallationRepositoriesAsync(string installationId, int perPage, int page, CancellationToken ct = default);
    }
}
