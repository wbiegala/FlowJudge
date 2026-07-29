namespace FlowJudge.Common.Messaging.Publishing
{
    internal interface IOutboxMessagePublisher
    {
        Task PublishOutboxMessageAsync(
            Guid messageId,
            string messageType,
            byte[] messagePayload,
            string publishSubject,
            CancellationToken cancellationToken);
    }
}
