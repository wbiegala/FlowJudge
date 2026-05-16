using FlowJudge.API.Contracts.Shared;

namespace FlowJudge.API.Contracts.Integrations
{
    public sealed record GetIntegrationsResponseItem
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Provider { get; init; }
        public required string Status { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public required UserData CreatedBy { get; init; }
    }
}
