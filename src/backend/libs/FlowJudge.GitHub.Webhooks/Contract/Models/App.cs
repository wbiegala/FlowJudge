using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record App
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("slug")]
        public string Slug { get; init; } = "";

        [JsonPropertyName("node_id")]
        public string NodeId { get; init; } = "";

        [JsonPropertyName("owner")]
        public User? Owner { get; init; }

        [JsonPropertyName("name")]
        public string Name { get; init; } = "";

        [JsonPropertyName("description")]
        public string? Description { get; init; }

        [JsonPropertyName("external_url")]
        public string ExternalUrl { get; init; } = "";

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; init; } = "";

        [JsonPropertyName("created_at")]
        public DateTimeOffset? CreatedAt { get; init; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; init; }

        [JsonPropertyName("permissions")]
        public Dictionary<string, string> Permissions { get; init; } = [];

        [JsonPropertyName("events")]
        public IReadOnlyCollection<string> Events { get; init; } = [];
    }
}
