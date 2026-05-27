using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record Commit
    {
        [JsonPropertyName("id")]
        public string Id { get; init; } = "";

        [JsonPropertyName("tree_id")]
        public string TreeId { get; init; } = "";

        [JsonPropertyName("distinct")]
        public bool Distinct { get; init; }

        [JsonPropertyName("message")]
        public string Message { get; init; } = "";

        [JsonPropertyName("timestamp")]
        public DateTimeOffset? Timestamp { get; init; }

        [JsonPropertyName("url")]
        public string Url { get; init; } = "";

        [JsonPropertyName("author")]
        public GitIdentity Author { get; init; } = new();

        [JsonPropertyName("committer")]
        public GitIdentity Committer { get; init; } = new();

        [JsonPropertyName("added")]
        public IReadOnlyCollection<string> Added { get; init; } = [];

        [JsonPropertyName("removed")]
        public IReadOnlyCollection<string> Removed { get; init; } = [];

        [JsonPropertyName("modified")]
        public IReadOnlyCollection<string> Modified { get; init; } = [];
    }

    public sealed record GitIdentity
    {
        [JsonPropertyName("name")]
        public string Name { get; init; } = "";

        [JsonPropertyName("email")]
        public string Email { get; init; } = "";

        [JsonPropertyName("username")]
        public string? Username { get; init; }
    }
}
