using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Webhooks.Contract.Models
{
    public sealed record BranchReference
    {
        [JsonPropertyName("label")]
        public string Label { get; init; } = "";

        [JsonPropertyName("ref")]
        public string Ref { get; init; } = "";

        [JsonPropertyName("sha")]
        public string Sha { get; init; } = "";

        [JsonPropertyName("user")]
        public User User { get; init; } = new();

        [JsonPropertyName("repo")]
        public Repository? Repository { get; init; }
    }
}
