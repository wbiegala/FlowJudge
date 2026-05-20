using FlowJudge.GitHub.Client.Contract.SharedModels;
using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Client.Contract
{
    public sealed record GetInstallationRepositoriesResponse
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; init; }

        [JsonPropertyName("repositories")]
        public IEnumerable<Repository> Repositories { get; init; } = Enumerable.Empty<Repository>();
    }
}
