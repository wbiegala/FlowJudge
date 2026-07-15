namespace FlowJudge.API.Contracts.Repositories
{
    public sealed record GetRepositoriesResponseItem
    {
        public Guid Id { get; init; }
        public required IntegrationModel Integration { get; init; }
        public Guid WorkspaceId { get; init; }
        public required string Name { get; init; }
        public string? FullName { get; init; }
        public bool TrackingEnabled { get; init; }
        public required string Status { get; init; }
    }
}
