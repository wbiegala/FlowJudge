namespace FlowJudge.VCS.Contracts.Events
{
    public sealed record Event<TEventData>
        where TEventData : class, IEventData
    {
        public Guid EventId { get; init; } = Guid.NewGuid();
        public VersionControlProvider Provider { get; init; }
        public EventType Type { get; init; }
        public required string ExternalEventId { get; init; }
        public required TEventData Data { get; init; }
    }
}
