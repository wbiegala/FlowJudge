using FlowJudge.GitHub.Client.Auth;

namespace FlowJudge.GitHub.Client.Clients.Impl
{
    internal abstract class InstallationTokenAuthenticatedClient
    {
        protected readonly HttpClient _httpClient;
        protected readonly IInstallationAuthService _installationAuthService;

        internal InstallationTokenAuthenticatedClient(
            HttpClient httpClient,
            IInstallationAuthService installationAuthService)
        {
            _httpClient = httpClient;
            _installationAuthService = installationAuthService;
        }
    }
}
