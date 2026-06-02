using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.Common.Messaging.Consumption
{
    internal sealed class ConsumerFactory : IConsumerFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ConsumerFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object GetConsumer(ConsumerOptions options)
        {
            return _serviceProvider.GetRequiredService(options.ConsumerType);
        }
    }
}
