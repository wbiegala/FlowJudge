namespace FlowJudge.Common.Domain
{
    public abstract class AggregateRoot : Entity
    {
        private readonly HashSet<DomainEvent> _domainEvents = new HashSet<DomainEvent>();

        protected void AddDomainEvent(DomainEvent @event)
        {
            _domainEvents.Add(@event);
        }

        public IReadOnlyCollection<DomainEvent> GetDomainEvents => _domainEvents;

        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
    }
}
