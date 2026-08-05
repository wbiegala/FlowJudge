using FlowJudge.Common.Domain;
using FlowJudge.Common.Messaging.Outbox;

namespace FlowJudge.Common.Messaging.Extensions
{
    public static class OutboxDomainEventsExtensions
    {
        public static async Task PublishDomainEventsAsync(
            this IOutbox outbox,
            AggregateRoot aggregate,
            CancellationToken cancellationToken = default)
        {
            var domainEvents = aggregate.GetDomainEvents;
            
            foreach (var @event in domainEvents)
            {
                if (@event is IMessage @publishable)
                {
                    await outbox.PublishAsync(@publishable, cancellationToken);
                }
                
            }
        }
    }
}
