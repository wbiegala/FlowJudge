namespace FlowJudge.GitHub.Client
{
    public interface IGitHubService
    {
        Task<string> GetInstallationUrlAsync(CancellationToken ct = default);

        TClient GetClient<TClient>()
            where TClient : class, IGitHubClient;
    }
}
