using FlowJudge.API.Service.Auth.Exceptions;
using FlowJudge.API.Service.Auth.Keycloak.Client.DTOs;
using System.Net;
using Constants = FlowJudge.API.Service.Auth.Common.Constants;

namespace FlowJudge.API.Service.Auth.Keycloak.Client
{
    internal sealed class KeycloakHttpClient : IKeycloakClient
    {
        private readonly KeycloakAuthenticationConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public KeycloakHttpClient(
            KeycloakAuthenticationConfiguration configuration,
            HttpClient httpClient)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<GetTokenResponse> GetTokenAsync(
            GrantType grantType,
            string value,
            string? redirectUrl = null,
            CancellationToken cancellationToken = default)
        {
            var form = BuildTokenRequestContent(grantType, value, redirectUrl);
            var endpoint = $"realms/{_configuration.Realm}/protocol/openid-connect/token";
            var content = new FormUrlEncodedContent(form);

            try
            {
                var response = await _httpClient.PostAsync(endpoint, content, cancellationToken);

                if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    var errorResponse = await response.Content.ReadFromJsonAsync<GetTokenErrorResponse>();
                    throw new TokenReceiveException($"Bad request for grantType={grantType.ToString()} and grantValue={value}",
                        errorResponse?.ErrorDescription ?? string.Empty);
                }

                response.EnsureSuccessStatusCode();

                var tokens = await response.Content.ReadFromJsonAsync<GetTokenResponse>();

                return tokens!;
            }
            catch (HttpRequestException ex)
            {
                throw new AuthenticationProviderException("Auth server error response", ex);
            }
            catch (Exception ex)
            {
                throw new AuthenticationProviderException("Auth server unavailable", ex);
            }
        }

        private IDictionary<string, string> BuildTokenRequestContent(GrantType grantType, string value, string? redirectUrl)
        {
            var content = new Dictionary<string, string>
            {
                { Constants.Authorization.ClientId, _configuration.ClientId },
                { Constants.Authorization.ClientSecret, _configuration.ClientSecret },
            };

            if (!string.IsNullOrWhiteSpace(redirectUrl))
            {
                content[Constants.Authorization.RedirectUri] = redirectUrl;
            }

            switch (grantType)
            {
                case GrantType.AuthorizationCode:
                    content.Add(Constants.GrantTypes.GRANT_TYPE, Constants.GrantTypes.AuthorizationCode);
                    content.Add(Constants.Authorization.Code, value);
                    break;
                case GrantType.RefreshToken:
                    content.Add(Constants.GrantTypes.GRANT_TYPE, Constants.GrantTypes.RefreshToken);
                    content.Add(Constants.Authorization.Refresh, value);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(grantType), grantType, null);
            }

            return content;
        }
    }
}
