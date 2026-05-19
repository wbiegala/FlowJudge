using FlowJudge.GitHub.Client.Auth.Contract;
using FlowJudge.GitHub.Client.Exceptions;
using System.Net.Http.Json;

namespace FlowJudge.GitHub.Client.Auth
{
    internal sealed class InstallationAuthHttpClient
    {
        private readonly HttpClient _httpClient;

        public InstallationAuthHttpClient(
            HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<(string InstallationToken, DateTimeOffset ExpireAt)> GetInstallationTokenAsync(
            string installationId, string accessToken, CancellationToken ct = default)
        {
            var url = $"app/installations/{installationId}/access_tokens";

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request, ct);
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadFromJsonAsync<GetInstallationTokenResponse>(ct);
            if (responseContent is null)
                throw new InvalidResponseException(url, typeof(GetInstallationTokenResponse));
            
            return new(responseContent.token, responseContent.expires_at);
        }
    }
}
