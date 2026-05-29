namespace FlowJudge.Common.Messaging.Consumption
{
    internal interface IConsumerFactory
    {
        IConsumer<TMessage> GetConsumer<TMessage>()
            where TMessage : class, IMessage;
    }
}
