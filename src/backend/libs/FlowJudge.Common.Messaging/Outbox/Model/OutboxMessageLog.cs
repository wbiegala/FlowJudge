namespace FlowJudge.Common.Messaging.Outbox.Model
{
    internal sealed record OutboxMessageLog
    {
        public Guid Id { get; init; }
        public Guid OutboxMessageId { get; init; }
        public DateTimeOffset Timestamp { get; init; }
        public OutboxMessageProcessingResult ProcessingResult { get; init; }
    }
}
