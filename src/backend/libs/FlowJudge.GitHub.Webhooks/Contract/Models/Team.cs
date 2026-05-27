using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record Team
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; init; } = "";

        [JsonPropertyName("name")]
        public string Name { get; init; } = "";

        [JsonPropertyName("slug")]
        public string Slug { get; init; } = "";

        [JsonPropertyName("description")]
        public string? Description { get; init; }

        [JsonPropertyName("privacy")]
        public string Privacy { get; init; } = "";

        [JsonPropertyName("notification_setting")]
        public string? NotificationSetting { get; init; }

        [JsonPropertyName("permission")]
        public string Permission { get; init; } = "";

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; init; } = "";

        [JsonPropertyName("url")]
        public string Url { get; init; } = "";
    }
}
