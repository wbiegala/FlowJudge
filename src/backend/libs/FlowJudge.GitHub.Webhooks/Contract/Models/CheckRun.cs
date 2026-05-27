using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record CheckRun
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; init; } = "";

        [JsonPropertyName("head_sha")]
        public string HeadSha { get; init; } = "";

        [JsonPropertyName("external_id")]
        public string ExternalId { get; init; } = "";

        [JsonPropertyName("url")]
        public string Url { get; init; } = "";

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; init; } = "";

        [JsonPropertyName("details_url")]
        public string? DetailsUrl { get; init; }

        [JsonPropertyName("status")]
        public string Status { get; init; } = "";

        [JsonPropertyName("conclusion")]
        public string? Conclusion { get; init; }

        [JsonPropertyName("started_at")]
        public DateTimeOffset? StartedAt { get; init; }

        [JsonPropertyName("completed_at")]
        public DateTimeOffset? CompletedAt { get; init; }

        [JsonPropertyName("output")]
        public CheckRunOutput Output { get; init; } = new();

        [JsonPropertyName("name")]
        public string Name { get; init; } = "";

        [JsonPropertyName("check_suite")]
        public CheckSuite CheckSuite { get; init; } = new();

        [JsonPropertyName("app")]
        public App? App { get; init; }

        [JsonPropertyName("pull_requests")]
        public IReadOnlyCollection<CheckPullRequest> PullRequests { get; init; } = [];

        [JsonPropertyName("head_commit")]
        public Commit? HeadCommit { get; init; }
    }

    public sealed record CheckRunOutput
    {
        [JsonPropertyName("title")]
        public string? Title { get; init; }

        [JsonPropertyName("summary")]
        public string? Summary { get; init; }

        [JsonPropertyName("text")]
        public string? Text { get; init; }

        [JsonPropertyName("annotations_count")]
        public long AnnotationsCount { get; init; }

        [JsonPropertyName("annotations_url")]
        public string AnnotationsUrl { get; init; } = "";
    }
}
