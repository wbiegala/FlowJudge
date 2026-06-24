namespace FlowJudge.Workspaces.Application.Abstractions.Models
{
    public sealed record RepositoryConfiguration
    {
        public Guid WorkspaceId { get; init; }
        public Guid IntegrationId { get; init; }
        public Guid? RepositoryId { get; init; }
        public required string ExternalId { get; init; }
        public required string Name { get; init; }
        public string? FullName { get; init; }
        public bool? TrackingEnabled { get; init; }
    }
}
