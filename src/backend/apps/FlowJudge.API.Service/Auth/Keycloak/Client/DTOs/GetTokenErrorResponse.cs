using System.Text.Json.Serialization;

namespace FlowJudge.API.Service.Auth.Keycloak.Client.DTOs
{
    public sealed record GetTokenErrorResponse
    {
        [JsonPropertyName("error")]
        public string Error { get; init; }

        [JsonPropertyName("error_description")]
        public string ErrorDescription { get; init; }
    }
}
