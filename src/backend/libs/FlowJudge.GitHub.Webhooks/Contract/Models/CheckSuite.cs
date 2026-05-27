using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record CheckSuite
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; init; } = "";

        [JsonPropertyName("head_branch")]
        public string? HeadBranch { get; init; }

        [JsonPropertyName("head_sha")]
        public string HeadSha { get; init; } = "";

        [JsonPropertyName("status")]
        public string Status { get; init; } = "";

        [JsonPropertyName("conclusion")]
        public string? Conclusion { get; init; }

        [JsonPropertyName("url")]
        public string Url { get; init; } = "";

        [JsonPropertyName("before")]
        public string? Before { get; init; }

        [JsonPropertyName("after")]
        public string? After { get; init; }

        [JsonPropertyName("pull_requests")]
        public IReadOnlyCollection<CheckPullRequest> PullRequests { get; init; } = [];

        [JsonPropertyName("app")]
        public App? App { get; init; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset? CreatedAt { get; init; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; init; }

        [JsonPropertyName("latest_check_runs_count")]
        public long? LatestCheckRunsCount { get; init; }

        [JsonPropertyName("check_runs_url")]
        public string CheckRunsUrl { get; init; } = "";

        [JsonPropertyName("head_commit")]
        public Commit? HeadCommit { get; init; }

        [JsonPropertyName("latest_check_runs")]
        public IReadOnlyCollection<CheckRun> LatestCheckRuns { get; init; } = [];
    }

    public sealed record CheckPullRequest
    {
        [JsonPropertyName("url")]
        public string Url { get; init; } = "";

        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("number")]
        public long Number { get; init; }

        [JsonPropertyName("head")]
        public CheckPullRequestBranch Head { get; init; } = new();

        [JsonPropertyName("base")]
        public CheckPullRequestBranch Base { get; init; } = new();
    }

    public sealed record CheckPullRequestBranch
    {
        [JsonPropertyName("ref")]
        public string Ref { get; init; } = "";

        [JsonPropertyName("sha")]
        public string Sha { get; init; } = "";

        [JsonPropertyName("repo")]
        public Repository? Repository { get; init; }
    }
}
