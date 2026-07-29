namespace FlowJudge.Common.Messaging.Outbox.SubjectMapping
{
    internal sealed record OutboxSubjectMapping
    {
        public required string TypeName { get; init; }
        public required string Subject { get; init; }
    }
}
