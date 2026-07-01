using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record Installation
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; init; } = "";

        [JsonPropertyName("account")]
        public Account? Account { get; init; }

        [JsonPropertyName("repository_selection")]
        public string RepositorySelection { get; init; } = "";

        [JsonPropertyName("app_id")]
        public long AppId { get; init; }

        [JsonPropertyName("app_slug")]
        public string AppSlug { get; init; } = "";

        [JsonPropertyName("target_id")]
        public long TargetId { get; init; }

        [JsonPropertyName("target_type")]
        public string TargetType { get; init; } = "";

        [JsonPropertyName("permissions")]
        public Dictionary<string, string> Permissions { get; init; } = [];

        [JsonPropertyName("events")]
        public IReadOnlyCollection<string> Events { get; init; } = [];

        [JsonPropertyName("created_at")]
        public DateTimeOffset CreatedAt { get; init; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset UpdatedAt { get; init; }

        [JsonPropertyName("suspended_at")]
        public DateTimeOffset? SuspendedAt { get; init; }
    }
}
