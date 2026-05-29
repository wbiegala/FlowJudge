namespace FlowJudge.Common.Messaging
{
    internal sealed record MessagingConfiguration
    {
        public required string ConnectionString { get; init; }
    }
}
