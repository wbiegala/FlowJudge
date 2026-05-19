using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FlowJudge.GitHub.Client.Contract.SharedModels
{
    public sealed record Repository
    {
        [JsonPropertyName("id")]
        public int Id { get; init; }

        [JsonPropertyName("name")]
        public required string Name { get; init; }

        [JsonPropertyName("full_name")]
        public required string FullName { get; init; }
    }
}
