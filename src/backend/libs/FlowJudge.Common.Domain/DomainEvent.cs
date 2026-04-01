namespace FlowJudge.Common.Domain
{
    public abstract record DomainEvent
    {
        public Guid EventId { get; init; }
    }
}
