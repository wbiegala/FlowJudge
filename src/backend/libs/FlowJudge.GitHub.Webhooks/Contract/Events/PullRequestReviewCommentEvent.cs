using FlowJudge.GitHub.Webhooks.Contract.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Events
{
    public sealed record PullRequestReviewCommentEvent
    {
        [JsonPropertyName("action")]
        public string Action { get; init; } = "";

        [JsonPropertyName("comment")]
        public PullRequestReviewComment Comment { get; init; } = new();

        [JsonPropertyName("pull_request")]
        public PullRequest PullRequest { get; init; } = new();

        [JsonPropertyName("changes")]
        public Dictionary<string, JsonElement> Changes { get; init; } = [];

        [JsonPropertyName("repository")]
        public Repository Repository { get; init; } = new();

        [JsonPropertyName("installation")]
        public Installation? Installation { get; init; }

        [JsonPropertyName("organization")]
        public Organization? Organization { get; init; }

        [JsonPropertyName("enterprise")]
        public Enterprise? Enterprise { get; init; }

        [JsonPropertyName("sender")]
        public User Sender { get; init; } = new();
    }
}
