using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record Organization
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; init; } = "";

        [JsonPropertyName("login")]
        public string Login { get; init; } = "";

        [JsonPropertyName("url")]
        public string Url { get; init; } = "";

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; init; } = "";

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; init; } = "";

        [JsonPropertyName("description")]
        public string? Description { get; init; }
    }
}
