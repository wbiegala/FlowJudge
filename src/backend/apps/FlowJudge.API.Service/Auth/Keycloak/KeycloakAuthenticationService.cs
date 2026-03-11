namespace FlowJudge.API.Service.Auth.Keycloak
{
    internal sealed class KeycloakAuthenticationService(
        KeycloakAuthenticationConfiguration configuration)
        : IAuthenticationService
    {
        public Task<string> GetRegistrationUrlAsync(CancellationToken cancellationToken = default)
        {
            var url = $"{configuration.BaseUrl}/realms/{configuration.Realm}/protocol/openid-connect/registrations";

            return Task.FromResult(url);
        }
    }
}
