namespace FlowJudge.API.Contracts.Integrations
{
    public sealed record UpdateIntegrationRequest
    {
        public required string Name { get; init; }
        public required string Status { get; init; }
        public IEnumerable<RepositoriesTrackingSettings> RepositoriesTrackingSettings { get; init; } = [];
    }

    public sealed record RepositoriesTrackingSettings
    {
        public Guid RepositoryId { get; init; }
        public bool TrackingEnabled { get; init; }
    }
}
