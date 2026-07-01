using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record PullRequestReview
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; init; } = "";

        [JsonPropertyName("user")]
        public User User { get; init; } = new();

        [JsonPropertyName("body")]
        public string? Body { get; init; }

        [JsonPropertyName("state")]
        public string State { get; init; } = "";

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; init; } = "";

        [JsonPropertyName("pull_request_url")]
        public string PullRequestUrl { get; init; } = "";

        [JsonPropertyName("submitted_at")]
        public DateTimeOffset? SubmittedAt { get; init; }

        [JsonPropertyName("commit_id")]
        public string CommitId { get; init; } = "";

        [JsonPropertyName("author_association")]
        public string AuthorAssociation { get; init; } = "";
    }
}
