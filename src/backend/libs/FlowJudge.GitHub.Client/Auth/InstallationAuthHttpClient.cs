using FlowJudge.GitHub.Client.Auth.Contract;
using FlowJudge.GitHub.Client.Exceptions;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

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
            string installationId,
            string appJwt,
            CancellationToken ct = default)
        {
            var url = $"app/installations/{installationId}/access_tokens";

            using var request = new HttpRequestMessage(HttpMethod.Post, url);

            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", appJwt);

            using var response = await _httpClient.SendAsync(request, ct);

            var body = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"GitHub installation token request failed. Status: {(int)response.StatusCode} {response.StatusCode}. Body: {body}");
            }

            var responseContent = JsonSerializer.Deserialize<GetInstallationTokenResponse>(
                body,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (responseContent is null)
            {
                throw new InvalidResponseException(url, typeof(GetInstallationTokenResponse));
            }

            return new(responseContent.token, responseContent.expires_at);
        }
    }
}
