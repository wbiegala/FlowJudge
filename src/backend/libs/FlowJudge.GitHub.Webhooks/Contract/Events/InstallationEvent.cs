using FlowJudge.GitHub.Webhooks.Contract.Models;
using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Events
{
    public sealed record InstallationEvent
    {
        [JsonPropertyName("action")]
        public string Action { get; init; } = "";

        [JsonPropertyName("installation")]
        public Installation Installation { get; init; } = new();

        [JsonPropertyName("repositories")]
        public IReadOnlyCollection<Repository> Repositories { get; init; } = [];

        [JsonPropertyName("requester")]
        public User? Requester { get; init; }

        [JsonPropertyName("sender")]
        public User Sender { get; init; } = new();

        [JsonPropertyName("organization")]
        public Organization? Organization { get; init; }

        [JsonPropertyName("enterprise")]
        public Enterprise? Enterprise { get; init; }
    }
}
