using FlowJudge.GitHub.Webhooks.Contract.Models;
using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Events
{
    public sealed record PushEvent
    {
        [JsonPropertyName("ref")]
        public string Ref { get; init; } = "";

        [JsonPropertyName("before")]
        public string Before { get; init; } = "";

        [JsonPropertyName("after")]
        public string After { get; init; } = "";

        [JsonPropertyName("created")]
        public bool Created { get; init; }

        [JsonPropertyName("deleted")]
        public bool Deleted { get; init; }

        [JsonPropertyName("forced")]
        public bool Forced { get; init; }

        [JsonPropertyName("base_ref")]
        public string? BaseRef { get; init; }

        [JsonPropertyName("compare")]
        public string Compare { get; init; } = "";

        [JsonPropertyName("commits")]
        public IReadOnlyCollection<Commit> Commits { get; init; } = [];

        [JsonPropertyName("head_commit")]
        public Commit? HeadCommit { get; init; }

        [JsonPropertyName("repository")]
        public Repository Repository { get; init; } = new();

        [JsonPropertyName("pusher")]
        public GitIdentity Pusher { get; init; } = new();

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
