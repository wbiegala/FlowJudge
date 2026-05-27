using FlowJudge.GitHub.Webhooks.Contract.Models;
using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Events
{
    public sealed record CheckSuiteEvent
    {
        [JsonPropertyName("action")]
        public string Action { get; init; } = "";

        [JsonPropertyName("check_suite")]
        public CheckSuite CheckSuite { get; init; } = new();

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
