using FlowJudge.Common.Http.Extensions;

namespace FlowJudge.API.Service.Auth.Keycloak
{
    internal static class KeycloakUrlHelper
    {
        public static string CreateRegistrationUrl(KeycloakAuthenticationConfiguration configuration, string clientOrigin)
        {
            var parameters = new RegistrationUrlParameters(
                client_id: configuration.ClientId,
                redirect_uri: CreateCallbackWithRedirectUrl(configuration.RegistrationCallbackUri, clientOrigin),
                response_type: "code",
                scope: "openid",
                prompt: "create");

            var baseUri = $"{configuration.BaseUrl}/realms/{configuration.Realm}/protocol/openid-connect/auth";
            var uriBuilder = new UriBuilder(baseUri);
            uriBuilder.AddQueryParams(parameters);

            return uriBuilder.ToString();
        }

        public static string NormalizeRegistrationConfirmationUrl(KeycloakAuthenticationConfiguration configuration, string clientOrigin)
        {
            return $"{clientOrigin}/confirm-registration";
        }

        private static string CreateCallbackWithRedirectUrl(string backendCallbackEndpoint, string clientOrigin) =>
            $"{backendCallbackEndpoint}?{AuthQueryParams.UiContextUrlParamName}={clientOrigin}";
        

        private sealed record RegistrationUrlParameters(
            string client_id,
            string redirect_uri,
            string response_type,
            string scope,
            string prompt);
    }
}
