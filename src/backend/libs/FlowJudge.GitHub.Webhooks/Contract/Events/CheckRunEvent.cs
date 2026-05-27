using FlowJudge.GitHub.Webhooks.Contract.Models;
using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Events
{
    public sealed record CheckRunEvent
    {
        [JsonPropertyName("action")]
        public string Action { get; init; } = "";

        [JsonPropertyName("check_run")]
        public CheckRun CheckRun { get; init; } = new();

        [JsonPropertyName("requested_action")]
        public CheckRunRequestedAction? RequestedAction { get; init; }

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

    public sealed record CheckRunRequestedAction
    {
        [JsonPropertyName("identifier")]
        public string Identifier { get; init; } = "";
    }
}
