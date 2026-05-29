namespace FlowJudge.Common.Messaging.Consumption
{
    internal abstract record ConsumerOptions
    {
        public int MaxConcurrentCalls { get; init; } = 4;
        public bool AutoCompleteMessages { get; init; } = false;
        public int MaxAutoLockRenewalDurationSeconds { get; init; } = 300;
    }   

    internal sealed record QueueConsumerOptions : ConsumerOptions
    {
        public required string QueueName { get; init; }
    }

    internal sealed record SubscriptionConsumerOptions : ConsumerOptions
    {
        public required string TopicName { get; init; }
        public required string SubscriptionName { get; init; }
    }
}
