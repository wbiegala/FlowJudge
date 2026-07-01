using FlowJudge.GitHub.Webhooks.Contract.Models;
using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Events
{
    public sealed record PingEvent
    {
        [JsonPropertyName("zen")]
        public string Zen { get; init; } = "";

        [JsonPropertyName("hook_id")]
        public long HookId { get; init; }

        [JsonPropertyName("hook")]
        public Hook Hook { get; init; } = new();

        [JsonPropertyName("repository")]
        public Repository? Repository { get; init; }

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
