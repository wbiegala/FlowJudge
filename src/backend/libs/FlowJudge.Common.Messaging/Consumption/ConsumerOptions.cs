namespace FlowJudge.Common.Messaging.Consumption
{
    internal abstract record ConsumerOptions
    {
        public required Type ConsumerType { get; init; }
        public required Type MessageType { get; init; }
        public int MaxConcurrentCalls { get; init; } = 4;
        public bool AutoCompleteMessages { get; init; } = false;
        public int MaxAutoLockRenewalDurationSeconds { get; init; } = 300;
        public abstract string ConsumerKey { get; }
    }   

    internal sealed record QueueConsumerOptions : ConsumerOptions
    {
        public required string QueueName { get; init; }
        public override string ConsumerKey => QueueName;
    }

    internal sealed record SubscriptionConsumerOptions : ConsumerOptions
    {
        public required string TopicName { get; init; }
        public required string SubscriptionName { get; init; }
        public override string ConsumerKey => $"{TopicName}/{SubscriptionName}";
    }
}
