using FlowJudge.API.Service.Auth.Keycloak;

namespace FlowJudge.API.Service.Auth
{
    public sealed class AuthenticationConfiguration
    {
        private readonly IDictionary<string, string> _configurationParameters = new Dictionary<string, string>();

        public void UseKeycloak(
            string baseUrl,
            string baseUrlInternal,
            string realm,
            string clientId,
            string clientSecret,
            string registrationCallbackUri,
            string loginCallbackUri,
            string logoutCallbackUri)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("BaseUrl must be provided.", nameof(baseUrl));
            if (string.IsNullOrWhiteSpace(baseUrlInternal))
                throw new ArgumentException("BaseUrlInternal must be provided.", nameof(baseUrlInternal));
            if (string.IsNullOrWhiteSpace(realm))
                throw new ArgumentException("Realm must be provided.", nameof(realm));
            if (string.IsNullOrWhiteSpace(clientId))
                throw new ArgumentException("ClientId must be provided.", nameof(clientId));
            if (string.IsNullOrWhiteSpace(clientSecret))
                throw new ArgumentException("ClientSecret must be provided.", nameof(clientSecret));
            if (string.IsNullOrWhiteSpace(registrationCallbackUri))
                throw new ArgumentException("RegistrationCallbackUri must be provided.", nameof(registrationCallbackUri));
            if (string.IsNullOrWhiteSpace(loginCallbackUri))
                throw new ArgumentException("RegistrationCallbackUri must be provided.", nameof(loginCallbackUri));
            if (string.IsNullOrWhiteSpace(logoutCallbackUri))
                throw new ArgumentException("LogoutCallbackUri must be provided.", nameof(logoutCallbackUri));

            _configurationParameters["AuthProvider"] = "Keycloak";
            _configurationParameters[KeycloakAuthenticationConfiguration.BaseUrlParameter] = baseUrl;
            _configurationParameters[KeycloakAuthenticationConfiguration.BaseUrlInternalParameter] = baseUrlInternal;
            _configurationParameters[KeycloakAuthenticationConfiguration.RealmParameter] = realm;
            _configurationParameters[KeycloakAuthenticationConfiguration.ClientIdParameter] = clientId;
            _configurationParameters[KeycloakAuthenticationConfiguration.ClientSecretParameter] = clientSecret;
            _configurationParameters[KeycloakAuthenticationConfiguration.RegistrationCallbackUriParameter] = registrationCallbackUri;
            _configurationParameters[KeycloakAuthenticationConfiguration.LoginCallbackUriParameter] = loginCallbackUri;
            _configurationParameters[KeycloakAuthenticationConfiguration.LogoutCallbackUriParameter] = logoutCallbackUri;
        }

        internal KeycloakAuthenticationConfiguration BuildKeycloakConfiguration()
        {
            return new KeycloakAuthenticationConfiguration
            {
                BaseUrl = _configurationParameters[KeycloakAuthenticationConfiguration.BaseUrlParameter],
                BaseUrlInternal = _configurationParameters[KeycloakAuthenticationConfiguration.BaseUrlInternalParameter],
                Realm = _configurationParameters[KeycloakAuthenticationConfiguration.RealmParameter],
                ClientId = _configurationParameters[KeycloakAuthenticationConfiguration.ClientIdParameter],
                ClientSecret = _configurationParameters[KeycloakAuthenticationConfiguration.ClientSecretParameter],
                RegistrationCallbackUri = _configurationParameters[KeycloakAuthenticationConfiguration.RegistrationCallbackUriParameter],
                LoginCallbackUri = _configurationParameters[KeycloakAuthenticationConfiguration.LoginCallbackUriParameter],
                LogoutCallbackUri = _configurationParameters[KeycloakAuthenticationConfiguration.LogoutCallbackUriParameter]
            };
        }
    }
}
