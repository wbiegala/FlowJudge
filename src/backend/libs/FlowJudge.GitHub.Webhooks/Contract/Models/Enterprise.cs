using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record Enterprise
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("slug")]
        public string Slug { get; init; } = "";

        [JsonPropertyName("name")]
        public string Name { get; init; } = "";

        [JsonPropertyName("node_id")]
        public string NodeId { get; init; } = "";

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; init; } = "";

        [JsonPropertyName("description")]
        public string? Description { get; init; }

        [JsonPropertyName("website_url")]
        public string? WebsiteUrl { get; init; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; init; } = "";

        [JsonPropertyName("created_at")]
        public DateTimeOffset? CreatedAt { get; init; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; init; }
    }
}
