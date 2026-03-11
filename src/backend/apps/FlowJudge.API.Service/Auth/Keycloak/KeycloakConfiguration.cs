namespace FlowJudge.API.Service.Auth.Keycloak
{
    internal class KeycloakAuthenticationConfiguration
    {
        public const string BaseUrlParameter = "BaseUrl";
        public const string BaseUrlInternalParameter = "BaseUrlInternal";
        public const string RealmParameter = "Realm";

        public required string BaseUrl { get; init; }
        public required string BaseUrlInternal { get; init; }
        public required string Realm { get; init; }
    }
}
