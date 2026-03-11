using FlowJudge.API.Service.Auth.Keycloak;

namespace FlowJudge.API.Service.Auth
{
    public sealed class AuthenticationConfiguration
    {
        private readonly IDictionary<string, string> _configurationParameters = new Dictionary<string, string>();

        public void UseKeycloak(
            string baseUrl,
            string baseUrlInternal,
            string realm)
        {
            if (string.IsNullOrWhiteSpace(baseUrl))
                throw new ArgumentException("BaseUrl must be provided.", nameof(baseUrl));
            if (string.IsNullOrWhiteSpace(baseUrlInternal))
                throw new ArgumentException("BaseUrlInternal must be provided.", nameof(baseUrlInternal));
            if (string.IsNullOrWhiteSpace(realm))
                throw new ArgumentException("Realm must be provided.", nameof(realm));

            _configurationParameters["AuthProvider"] = "Keycloak";
            _configurationParameters[KeycloakAuthenticationConfiguration.BaseUrlParameter] = baseUrl;
            _configurationParameters[KeycloakAuthenticationConfiguration.BaseUrlInternalParameter] = baseUrlInternal;
            _configurationParameters[KeycloakAuthenticationConfiguration.RealmParameter] = realm;
        }

        internal KeycloakAuthenticationConfiguration BuildKeycloakConfiguration()
        {
            return new KeycloakAuthenticationConfiguration
            {
                BaseUrl = _configurationParameters[KeycloakAuthenticationConfiguration.BaseUrlParameter],
                BaseUrlInternal = _configurationParameters[KeycloakAuthenticationConfiguration.BaseUrlInternalParameter],
                Realm = _configurationParameters[KeycloakAuthenticationConfiguration.RealmParameter],
            };
        }
    }
}
