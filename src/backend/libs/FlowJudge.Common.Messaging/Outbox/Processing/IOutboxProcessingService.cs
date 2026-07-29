namespace FlowJudge.Common.Messaging.Outbox.Processing
{
    internal interface IOutboxProcessingService
    {
        Task ProcessBatchAsync(CancellationToken cancellationToken = default);
    }
}
