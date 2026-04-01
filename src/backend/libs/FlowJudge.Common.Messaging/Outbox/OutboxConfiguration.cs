namespace FlowJudge.Common.Messaging.Outbox
{
    internal sealed class OutboxConfiguration
    {
        public const string SchemaName = "public";

        public const string OutboxMessageTableName = "outbox_messages";
        public const string OutboxMessageLogTableName = "outbox_message_logs";
    }
}
