using FlowJudge.Common.Messaging.Outbox.SubjectMapping;

namespace FlowJudge.Common.Messaging.Outbox
{
    internal sealed class OutboxConfiguration
    {
        public const string SchemaName = "public";
        public const string OutboxMessageTableName = "outbox_messages";
        public const string OutboxMessageLogTableName = "outbox_message_logs";

        public int MaxRetryCount { get; init; } = 5;
        public int ProcessingBatchSize { get; init; } = 100;
        public int ProcessingIntervalInSeconds { get; init; } = 10;
        public IReadOnlyCollection<OutboxSubjectMapping> SubjectMapping { get; init; } = Array.Empty<OutboxSubjectMapping>();
    }
}
