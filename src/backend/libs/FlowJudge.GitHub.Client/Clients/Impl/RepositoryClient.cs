using FlowJudge.GitHub.Client.Auth;
using FlowJudge.GitHub.Client.Contract;
using FlowJudge.GitHub.Client.Exceptions;
using System.Net.Http.Json;

namespace FlowJudge.GitHub.Client.Clients.Impl
{
    internal sealed class RepositoryClient : InstallationTokenAuthenticatedClient, IRepositoryClient
    {
        public RepositoryClient(HttpClient httpClient, IInstallationAuthService installationAuthService)
            : base(httpClient, installationAuthService)
        {
        }

        public async Task<GetInstallationRepositoriesResponse> GetInstallationRepositoriesAsync(string installationId, int perPage, int page, CancellationToken ct = default)
        {
            const string baseUrl = "/installation/repositories";
            var url = $"{baseUrl}?perPage={perPage}&page={page}";

            var request = new HttpRequestMessage(HttpMethod.Get, baseUrl);
            var authToken = await _installationAuthService.GetInstallationTokenAsync(installationId, ct);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", authToken);

            var resposne = await _httpClient.SendAsync(request, ct);
            resposne.EnsureSuccessStatusCode();
            var result = await resposne.Content.ReadFromJsonAsync<GetInstallationRepositoriesResponse>(ct);

            if (result is null)
                throw new InvalidResponseException(installationId, url, typeof(GetInstallationRepositoriesResponse));

            return result;
        }
    }
}
