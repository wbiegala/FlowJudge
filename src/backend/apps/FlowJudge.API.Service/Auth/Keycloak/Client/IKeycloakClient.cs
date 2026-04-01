using FlowJudge.API.Service.Auth.Keycloak.Client.DTOs;

namespace FlowJudge.API.Service.Auth.Keycloak.Client
{
    internal interface IKeycloakClient
    {
        Task<GetTokenResponse> GetTokenAsync(
            GrantType grantType,
            string value,
            string? redirectUrl = null,
            CancellationToken cancellationToken = default);
    }
}
