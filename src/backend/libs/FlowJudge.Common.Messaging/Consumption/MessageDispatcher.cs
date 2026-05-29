using Azure.Messaging.ServiceBus;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowJudge.Common.Messaging.Consumption
{
    internal sealed class MessageDispatcher
    {
        private readonly IConsumerFactory _consumerFactory;

        public MessageDispatcher(IConsumerFactory consumerFactory)
        {
            _consumerFactory = consumerFactory;
        }

        public async Task DispatchAsync(ServiceBusReceivedMessage message, CancellationToken ct)
        {
            
        }
    }
}
