using FlowJudge.GitHub.Webhooks.Contract.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Events
{
    public sealed record PullRequestEvent
    {
        [JsonPropertyName("action")]
        public string Action { get; init; } = "";

        [JsonPropertyName("number")]
        public long Number { get; init; }

        [JsonPropertyName("pull_request")]
        public PullRequest PullRequest { get; init; } = new();

        [JsonPropertyName("changes")]
        public Dictionary<string, JsonElement> Changes { get; init; } = [];

        [JsonPropertyName("requested_reviewer")]
        public User? RequestedReviewer { get; init; }

        [JsonPropertyName("requested_team")]
        public Team? RequestedTeam { get; init; }

        [JsonPropertyName("assignee")]
        public User? Assignee { get; init; }

        [JsonPropertyName("label")]
        public Label? Label { get; init; }

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
