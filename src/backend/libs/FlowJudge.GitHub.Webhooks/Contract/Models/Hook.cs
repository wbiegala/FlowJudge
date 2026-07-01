using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record Hook
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("type")]
        public string Type { get; init; } = "";

        [JsonPropertyName("name")]
        public string Name { get; init; } = "";

        [JsonPropertyName("active")]
        public bool Active { get; init; }

        [JsonPropertyName("events")]
        public IReadOnlyCollection<string> Events { get; init; } = [];

        [JsonPropertyName("config")]
        public Dictionary<string, string?> Config { get; init; } = [];

        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; init; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset? CreatedAt { get; init; }

        [JsonPropertyName("app_id")]
        public long? AppId { get; init; }
    }
}
