using FlowJudge.API.Service.Consumers;
using FlowJudge.Common.Messaging;
using FlowJudge.VCS.Contracts.Events;

namespace FlowJudge.API.Service.Installers
{
    public static class ServiceBusInstaller
    {
        public static void InstallServiceBus(this WebApplicationBuilder builder)
        {
            var connectionString = builder.Configuration.GetConnectionString("ServiceBus");

            if (string.IsNullOrWhiteSpace(connectionString))
                throw new InvalidOperationException("Service Bus connection string is not configured.");

            builder.Services.AddAzureServiceBus(cfg =>
            {
                cfg.WithConnectionString(connectionString);
                cfg.WithConsumers(c =>
                {
                    c.AddConsumer<IntegrationChangedEventConsumer, IntegrationChangedEvent>(
                        Topics.IntegrationEventTopic,
                        "flow-judge.api.integration-changed-event");
                });
            });
        }
    }
}
