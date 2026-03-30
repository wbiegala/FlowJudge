namespace FlowJudge.Common.Messaging.Outbox
{
    public interface IOutbox
    {
        Task PublishAsync(IMessage message, CancellationToken cancellationToken = default);
    }
}
