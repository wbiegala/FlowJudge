using FlowJudge.Common.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;

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

        public static string CreateLoginUrl(KeycloakAuthenticationConfiguration configuration,
            string clientId, 
            string redirectUri,
            string responseType,
            string scope,
            string state)
        {
            var parameters = new LoginUrlParameters(
                client_id: clientId,
                redirect_uri: redirectUri,
                response_type: responseType,
                scope: scope,
                state: state);

            var baseUri = $"{configuration.BaseUrl}/realms/{configuration.Realm}/protocol/openid-connect/auth";
            var uriBuilder = new UriBuilder(baseUri);
            uriBuilder.AddQueryParams(parameters);

            return uriBuilder.ToString();
        }

        public static string CreateCallbackWithRedirectUrl(
            string backendCallbackEndpoint,
            string uiRedirectUri)
        {
            if (string.IsNullOrWhiteSpace(backendCallbackEndpoint))
                throw new ArgumentException("Backend callback endpoint cannot be null or empty.", nameof(backendCallbackEndpoint));

            if (string.IsNullOrWhiteSpace(uiRedirectUri))
                throw new ArgumentException("UI redirect url cannot be null or empty.", nameof(uiRedirectUri));

            if (!Uri.TryCreate(backendCallbackEndpoint, UriKind.Absolute, out var callbackUri))
                throw new ArgumentException("Backend callback endpoint must be a valid absolute URL.", nameof(backendCallbackEndpoint));

            if (!Uri.TryCreate(uiRedirectUri, UriKind.Absolute, out _))
                throw new ArgumentException("UI redirect url must be a valid absolute URL.", nameof(uiRedirectUri));

            return QueryHelpers.AddQueryString(
                callbackUri.ToString(),
                AuthQueryParams.UiContextUrlParamName,
                uiRedirectUri);
        }

        public static string CreateLogoutUrl(KeycloakAuthenticationConfiguration configuration,
            string identityToken,
            string uiContext)
        {
            var baseUri = $"{configuration.BaseUrl}/realms/{configuration.Realm}/protocol/openid-connect/logout";
            var redirectUri = CreateCallbackWithRedirectUrl(configuration.LogoutCallbackUri, uiContext);
            var parameters = new LogoutUrlParameters(
                id_token_hint: identityToken,
                post_logout_redirect_uri: redirectUri);

            var uriBuilder = new UriBuilder(baseUri);
            uriBuilder.AddQueryParams(parameters);

            return uriBuilder.ToString();
        }


        private sealed record RegistrationUrlParameters(
            string client_id,
            string redirect_uri,
            string response_type,
            string scope,
            string prompt);

        private sealed record LoginUrlParameters(
            string client_id,
            string redirect_uri,
            string response_type,
            string scope,
            string state);

        private sealed record LogoutUrlParameters(
            string id_token_hint,
            string post_logout_redirect_uri,
            string? state = null);
    }
}
