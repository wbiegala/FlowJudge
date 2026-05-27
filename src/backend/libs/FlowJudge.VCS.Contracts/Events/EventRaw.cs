namespace FlowJudge.VCS.Contracts.Events
{
    public sealed record EventRaw
    {
        public VersionControlProvider Provider { get; init; }
        public required string ExternalEventId { get; init; }
        public required string Data { get; init; }
    }
}
