using FlowJudge.VCS.Contracts.Events;

namespace FlowJudge.VCS.Worker.EventPublishing
{
    public interface IEventPublisher
    {
        Task PublishAsync<TEventData>(Event<TEventData> @event, CancellationToken cancellationToken = default)
            where TEventData : class, IEventData;
    }
}
