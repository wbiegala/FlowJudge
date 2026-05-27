using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record PullRequest
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; init; } = "";

        [JsonPropertyName("url")]
        public string Url { get; init; } = "";

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; init; } = "";

        [JsonPropertyName("diff_url")]
        public string DiffUrl { get; init; } = "";

        [JsonPropertyName("patch_url")]
        public string PatchUrl { get; init; } = "";

        [JsonPropertyName("issue_url")]
        public string IssueUrl { get; init; } = "";

        [JsonPropertyName("number")]
        public long Number { get; init; }

        [JsonPropertyName("state")]
        public string State { get; init; } = "";

        [JsonPropertyName("locked")]
        public bool Locked { get; init; }

        [JsonPropertyName("title")]
        public string Title { get; init; } = "";

        [JsonPropertyName("user")]
        public User User { get; init; } = new();

        [JsonPropertyName("body")]
        public string? Body { get; init; }

        [JsonPropertyName("created_at")]
        public DateTimeOffset? CreatedAt { get; init; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; init; }

        [JsonPropertyName("closed_at")]
        public DateTimeOffset? ClosedAt { get; init; }

        [JsonPropertyName("merged_at")]
        public DateTimeOffset? MergedAt { get; init; }

        [JsonPropertyName("merge_commit_sha")]
        public string? MergeCommitSha { get; init; }

        [JsonPropertyName("assignee")]
        public User? Assignee { get; init; }

        [JsonPropertyName("assignees")]
        public IReadOnlyCollection<User> Assignees { get; init; } = [];

        [JsonPropertyName("requested_reviewers")]
        public IReadOnlyCollection<User> RequestedReviewers { get; init; } = [];

        [JsonPropertyName("requested_teams")]
        public IReadOnlyCollection<Team> RequestedTeams { get; init; } = [];

        [JsonPropertyName("labels")]
        public IReadOnlyCollection<Label> Labels { get; init; } = [];

        [JsonPropertyName("milestone")]
        public Milestone? Milestone { get; init; }

        [JsonPropertyName("draft")]
        public bool Draft { get; init; }

        [JsonPropertyName("commits")]
        public long Commits { get; init; }

        [JsonPropertyName("additions")]
        public long Additions { get; init; }

        [JsonPropertyName("deletions")]
        public long Deletions { get; init; }

        [JsonPropertyName("changed_files")]
        public long ChangedFiles { get; init; }

        [JsonPropertyName("head")]
        public BranchReference Head { get; init; } = new();

        [JsonPropertyName("base")]
        public BranchReference Base { get; init; } = new();

        [JsonPropertyName("author_association")]
        public string AuthorAssociation { get; init; } = "";

        [JsonPropertyName("auto_merge")]
        public AutoMerge? AutoMerge { get; init; }

        [JsonPropertyName("merged")]
        public bool? Merged { get; init; }

        [JsonPropertyName("mergeable")]
        public bool? Mergeable { get; init; }

        [JsonPropertyName("rebaseable")]
        public bool? Rebaseable { get; init; }
    }

    public sealed record AutoMerge
    {
        [JsonPropertyName("enabled_by")]
        public User EnabledBy { get; init; } = new();

        [JsonPropertyName("merge_method")]
        public string MergeMethod { get; init; } = "";

        [JsonPropertyName("commit_title")]
        public string CommitTitle { get; init; } = "";

        [JsonPropertyName("commit_message")]
        public string CommitMessage { get; init; } = "";
    }
}
