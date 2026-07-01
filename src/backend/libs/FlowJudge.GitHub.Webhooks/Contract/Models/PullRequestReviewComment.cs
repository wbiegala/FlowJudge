using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record PullRequestReviewComment
    {
        [JsonPropertyName("url")]
        public string Url { get; init; } = "";

        [JsonPropertyName("pull_request_review_id")]
        public long? PullRequestReviewId { get; init; }

        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; init; } = "";

        [JsonPropertyName("diff_hunk")]
        public string DiffHunk { get; init; } = "";

        [JsonPropertyName("path")]
        public string Path { get; init; } = "";

        [JsonPropertyName("position")]
        public long? Position { get; init; }

        [JsonPropertyName("original_position")]
        public long? OriginalPosition { get; init; }

        [JsonPropertyName("commit_id")]
        public string CommitId { get; init; } = "";

        [JsonPropertyName("original_commit_id")]
        public string OriginalCommitId { get; init; } = "";

        [JsonPropertyName("user")]
        public User User { get; init; } = new();

        [JsonPropertyName("body")]
        public string Body { get; init; } = "";

        [JsonPropertyName("created_at")]
        public DateTimeOffset? CreatedAt { get; init; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; init; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; init; } = "";

        [JsonPropertyName("pull_request_url")]
        public string PullRequestUrl { get; init; } = "";

        [JsonPropertyName("author_association")]
        public string AuthorAssociation { get; init; } = "";

        [JsonPropertyName("start_line")]
        public long? StartLine { get; init; }

        [JsonPropertyName("original_start_line")]
        public long? OriginalStartLine { get; init; }

        [JsonPropertyName("start_side")]
        public string? StartSide { get; init; }

        [JsonPropertyName("line")]
        public long? Line { get; init; }

        [JsonPropertyName("original_line")]
        public long? OriginalLine { get; init; }

        [JsonPropertyName("side")]
        public string? Side { get; init; }

        [JsonPropertyName("in_reply_to_id")]
        public long? InReplyToId { get; init; }
    }
}
