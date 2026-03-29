using System.Text.Json.Serialization;

namespace FlowJudge.API.Service.Auth.Keycloak.Client.DTOs
{
    public sealed record GetTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }

        [JsonPropertyName("refresh_expires_in")]
        public int RefreshExpiresIn { get; init; }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken { get; init; }

        [JsonPropertyName("id_token")]
        public string IdentityToken { get; init; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; init; }

        [JsonPropertyName("not-before-policy")]
        public int NotBeforePolicy { get; init; }

        [JsonPropertyName("session_state")]
        public string SessionState { get; init; }

        [JsonPropertyName("scope")]
        public string Scope { get; init; }
    }
}
