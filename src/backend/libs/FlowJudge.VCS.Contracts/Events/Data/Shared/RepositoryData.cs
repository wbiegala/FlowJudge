namespace FlowJudge.VCS.Contracts.Events.Data.Shared
{
    public sealed record RepositoryData
    {
        public required string Id { get; init; }
        public required string Name { get; init; }
        public string? FullName { get; init; }
    }
}
