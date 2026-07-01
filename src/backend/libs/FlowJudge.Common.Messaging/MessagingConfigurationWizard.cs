using FlowJudge.Common.Messaging.Consumption;
using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.Common.Messaging
{
    public sealed class MessagingConfigurationWizard
    {
        private string _azureServiceBusConnectionString = string.Empty;
        private readonly MessagingConsumersBuilder _consumersBuilder = new();

        public void WithConnectionString(string connectionString) => _azureServiceBusConnectionString = connectionString;

        public void WithConsumers(Action<MessagingConsumersBuilder> configure)
        {
            configure(_consumersBuilder);
        }

        internal MessagingConfiguration GetConfiguration() =>
            new MessagingConfiguration
            {
                ConnectionString = _azureServiceBusConnectionString,
            };

        internal IReadOnlyCollection<(ConsumerOptions Options, Action<IServiceCollection> Registration)> ConsumersOptions
            => _consumersBuilder.ConsumersOptions;

        public sealed class MessagingConsumersBuilder
        {
            private readonly List<(ConsumerOptions Options, Action<IServiceCollection> Registration)> _consumerOptions = new();

            internal IReadOnlyCollection<(ConsumerOptions Options, Action<IServiceCollection> Registration)> ConsumersOptions 
                => _consumerOptions;

            public void AddConsumer<TConsumer, TMessage>(
                string queueName,
                int maxConcurrentCalls = 4,
                bool autoCompleteMessages = false,
                int maxAutoLockRenewalDurationSeconds = 300)
                    where TConsumer : class, IConsumer<TMessage>
                    where TMessage : class, IMessage
            {
                var options = new QueueConsumerOptions
                {
                    QueueName = queueName,
                    ConsumerType = typeof(TConsumer),
                    MessageType = typeof(TMessage),
                    MaxConcurrentCalls = maxConcurrentCalls,
                    AutoCompleteMessages = autoCompleteMessages,
                    MaxAutoLockRenewalDurationSeconds = maxAutoLockRenewalDurationSeconds,
                };

                _consumerOptions.Add(new (options, services =>
                {
                    services.AddScoped<TConsumer>();
                    services.AddScoped<IConsumer<TMessage>>(sp => sp.GetRequiredService<TConsumer>());
                }));
            }

            public void AddConsumer<TConsumer, TMessage>(
                string topicName,
                string subscriptionName,
                int maxConcurrentCalls = 4,
                bool autoCompleteMessages = false,
                int maxAutoLockRenewalDurationSeconds = 300)
                    where TConsumer : class, IConsumer<TMessage>
                    where TMessage : class, IMessage
            {
                var options = new SubscriptionConsumerOptions
                {
                    TopicName = topicName,
                    SubscriptionName = subscriptionName,
                    ConsumerType = typeof(TConsumer),
                    MessageType = typeof(TMessage),
                    MaxConcurrentCalls = maxConcurrentCalls,
                    AutoCompleteMessages = autoCompleteMessages,
                    MaxAutoLockRenewalDurationSeconds = maxAutoLockRenewalDurationSeconds,
                };

                _consumerOptions.Add(new (options, services =>
                {
                    services.AddScoped<TConsumer>();
                    services.AddScoped<IConsumer<TMessage>>(sp => sp.GetRequiredService<TConsumer>());
                }));
            }
        }
    }
}
