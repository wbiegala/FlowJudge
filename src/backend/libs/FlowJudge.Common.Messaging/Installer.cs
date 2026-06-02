using Azure.Messaging.ServiceBus;
using FlowJudge.Common.Messaging.Abstractions;
using FlowJudge.Common.Messaging.Consumption;
using FlowJudge.Common.Messaging.Outbox;
using FlowJudge.Common.Messaging.Outbox.Impl;
using FlowJudge.Common.Messaging.Publishing;
using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.Common.Messaging
{
    public static class Installer
    {
        public static IServiceCollection AddOutbox(this IServiceCollection services)
        {
            services.AddScoped<IOutbox, PostgresOutbox>();

            return services;
        }

        public static IServiceCollection AddAzureServiceBus(
            this IServiceCollection services,
            Action<MessagingConfigurationWizard> configure)
        {
            var cfgBuilder = new MessagingConfigurationWizard();
            configure(cfgBuilder);

            var configuration = cfgBuilder.GetConfiguration();
            services.AddSingleton(_ => configuration);
            services.AddSingleton(_ => new ServiceBusClient(configuration.ConnectionString));
            services.AddSingleton<IPublisher, MessagePublisher>();

            var consumersOptions = cfgBuilder.ConsumersOptions;

            if (consumersOptions.Count > 0)
            {
                services.AddSingleton<MessageDispatcher>();
                services.AddScoped<IConsumerFactory, ConsumerFactory>();
                
                var duplicatedConsumerKey = consumersOptions
                    .GroupBy(x => x.Options.ConsumerKey, StringComparer.OrdinalIgnoreCase)
                    .FirstOrDefault(x => x.Count() > 1);

                if (duplicatedConsumerKey is not null)
                {
                    throw new InvalidOperationException(
                        $"A consumer for '{duplicatedConsumerKey.Key}' is already registered.");
                }

                foreach (var options in consumersOptions)
                {
                    options.Registration(services);
                    services.AddSingleton(options.Options);
                }

                services.AddHostedService<MessageProcessingService>();
            }

            return services;
        }
    }
}
