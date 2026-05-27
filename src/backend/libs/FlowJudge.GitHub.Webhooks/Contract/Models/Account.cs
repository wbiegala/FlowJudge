using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record Account
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("login")]
        public string Login { get; init; } = "";

        [JsonPropertyName("type")]
        public string Type { get; init; } = "";
    }
}
