using FlowJudge.API.Contracts.Repositories;
using FlowJudge.API.Contracts.Shared;

namespace FlowJudge.API.Contracts.Integrations
{
    public sealed record GetIntegrationDetailsResponse
    {
        public Guid Id { get; init; }
        public Guid WorkspaceId { get; init; }
        public required string Name { get; init; }
        public required string Provider { get; init; }
        public required string Status { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public required UserData CreatedBy { get; init; }
        public required IEnumerable<GetRepositoriesResponseItem> Repositories { get; init; }
    }
}
