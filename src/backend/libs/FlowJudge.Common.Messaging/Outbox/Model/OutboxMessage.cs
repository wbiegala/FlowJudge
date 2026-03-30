namespace FlowJudge.Common.Messaging.Outbox.Model
{
    internal sealed record OutboxMessage
    {
        public Guid Id { get; init; }
        public required string Type { get; init; }
        public Guid SystemId { get; init; }
        public required byte[] Payload { get; init; }
        public DateTimeOffset PublicationTimestamp { get; init; }
        public DateTimeOffset? ProcessingTimestamp { get; init; }
    }
}
