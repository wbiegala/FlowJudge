using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record Repository
    {
        [JsonPropertyName("id")]
        public long Id { get; init; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; init; } = "";

        [JsonPropertyName("name")]
        public string Name { get; init; } = "";

        [JsonPropertyName("full_name")]
        public string FullName { get; init; } = "";

        [JsonPropertyName("private")]
        public bool IsPrivate { get; init; }

        [JsonPropertyName("owner")]
        public User Owner { get; init; } = new();

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; init; } = "";

        [JsonPropertyName("description")]
        public string? Description { get; init; }

        [JsonPropertyName("fork")]
        public bool Fork { get; init; }

        [JsonPropertyName("url")]
        public string Url { get; init; } = "";

        [JsonPropertyName("created_at")]
        public DateTimeOffset? CreatedAt { get; init; }

        [JsonPropertyName("updated_at")]
        public DateTimeOffset? UpdatedAt { get; init; }

        [JsonPropertyName("pushed_at")]
        public DateTimeOffset? PushedAt { get; init; }

        [JsonPropertyName("clone_url")]
        public string CloneUrl { get; init; } = "";

        [JsonPropertyName("ssh_url")]
        public string SshUrl { get; init; } = "";

        [JsonPropertyName("default_branch")]
        public string DefaultBranch { get; init; } = "";

        [JsonPropertyName("language")]
        public string? Language { get; init; }

        [JsonPropertyName("archived")]
        public bool Archived { get; init; }

        [JsonPropertyName("disabled")]
        public bool? Disabled { get; init; }

        [JsonPropertyName("visibility")]
        public string Visibility { get; init; } = "";
    }
}
