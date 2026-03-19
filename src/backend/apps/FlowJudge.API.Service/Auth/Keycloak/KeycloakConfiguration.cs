namespace FlowJudge.API.Service.Auth.Keycloak
{
    internal class KeycloakAuthenticationConfiguration
    {
        public const string BaseUrlParameter = "BaseUrl";
        public const string BaseUrlInternalParameter = "BaseUrlInternal";
        public const string RealmParameter = "Realm";
        public const string ClientIdParameter = "ClientId";
        public const string ClientSecretParameter = "ClientSecret";
        public const string RegistrationCallbackUriParameter = "RegistrationCallbackUri";

        public required string BaseUrl { get; init; }
        public required string BaseUrlInternal { get; init; }
        public required string Realm { get; init; }
        public required string ClientId { get; init; }
        public required string ClientSecret { get; init; }
        public required string RegistrationCallbackUri { get; init; }
    }
}
