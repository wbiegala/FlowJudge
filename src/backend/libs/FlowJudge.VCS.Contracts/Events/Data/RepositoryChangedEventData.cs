namespace FlowJudge.VCS.Contracts.Events.Data
{
    public sealed record RepositoryChangedEventData : IEventData
    {
        public required string IntegrationId { get; init; }
        public RepositoryAction Action { get; init; }
        public required string RepositoryId { get; init; }
        public required string RepositoryName { get; init; }
        public string? RepositoryFullName { get; init; }

        public enum RepositoryAction
        {
            AccessGranted,
            AccessRevoked,
        }
    }
}
