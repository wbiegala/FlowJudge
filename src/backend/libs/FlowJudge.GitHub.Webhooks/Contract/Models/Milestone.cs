using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record Milestone
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; init; } = "";

        [JsonPropertyName("number")]
        public long Number { get; init; }

        [JsonPropertyName("title")]
        public string Title { get; init; } = "";

        [JsonPropertyName("description")]
        public string? Description { get; init; }

        [JsonPropertyName("state")]
        public string State { get; init; } = "";

        [JsonPropertyName("created_at")]
        public DateTimeOffset? CreatedAt { get; init; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; init; }

        [JsonPropertyName("due_on")]
        public DateTimeOffset? DueOn { get; init; }

        [JsonPropertyName("closed_at")]
        public DateTimeOffset? ClosedAt { get; init; }
    }
}
