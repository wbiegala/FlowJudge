namespace FlowJudge.API.Service.Auth.Keycloak
{
    internal sealed class KeycloakAuthenticationService(
        KeycloakAuthenticationConfiguration configuration)
        : IAuthenticationService
    {
        public Task<string> GetRegistrationUrlAsync(string clientOrigin, CancellationToken cancellationToken = default)
        {
            var url = KeycloakUrlHelper.CreateRegistrationUrl(configuration, clientOrigin);

            return Task.FromResult(url);
        }

        public Task<string> GetRegistrationCallbackUrlAsync(string clientOrigin, CancellationToken cancellationToken = default)
        {
            var url = KeycloakUrlHelper.NormalizeRegistrationConfirmationUrl(configuration, clientOrigin);

            return Task.FromResult(url);
        }
    }
}
