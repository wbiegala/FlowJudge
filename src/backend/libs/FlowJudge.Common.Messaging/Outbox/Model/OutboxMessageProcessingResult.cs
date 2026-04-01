namespace FlowJudge.Common.Messaging.Outbox.Model
{
    internal enum OutboxMessageProcessingResult
    {
        Skipped = 0,
        Sent = 200,
        Error = 400
    }
}
