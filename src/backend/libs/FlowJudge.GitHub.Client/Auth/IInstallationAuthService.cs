namespace FlowJudge.GitHub.Client.Auth
{
    internal interface IInstallationAuthService
    {
        Task<string> GetInstallationTokenAsync(string installationId, CancellationToken ct = default);
    }
}
