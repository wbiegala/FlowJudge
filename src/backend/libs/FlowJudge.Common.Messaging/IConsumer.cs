namespace FlowJudge.Common.Messaging
{
    public interface IConsumer<TMessage>
        where TMessage : class, IMessage
    {
        Task ConsumeAsync(TMessage message, CancellationToken cancellationToken = default);
    }
}
