namespace FlowJudge.Common.Messaging.Consumption
{
    internal interface IConsumerFactory
    {
        object GetConsumer(ConsumerOptions options);
    }
}
