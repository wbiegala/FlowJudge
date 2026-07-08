namespace FlowJudge.API.Contracts.Repositories
{
    public sealed record IntegrationModel
    {
        public Guid IntegrationId { get; init; }
        public Guid WorkspaceId { get; init; }
        public required string Name { get; init; }
        public required string Provider { get; init; }
        public required string Status { get; init; }
    }
}
