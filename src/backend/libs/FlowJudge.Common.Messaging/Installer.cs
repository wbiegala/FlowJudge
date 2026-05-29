using FlowJudge.Common.Messaging.Outbox;
using FlowJudge.Common.Messaging.Outbox.Impl;
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

        public static IServiceCollection AddAzureSerivceBus(
            this IServiceCollection services,
            Action<MessagingConfigurationWizard> configure)
        {
            var cfgBuilder = new MessagingConfigurationWizard();
            configure(cfgBuilder);

            var configuration = cfgBuilder.GetConfiguration();
            services.AddSingleton(_ => configuration);

            var consumersOptions = cfgBuilder.ConsumersOptions;

            foreach (var options in consumersOptions)
            {
                options.Registration(services);
                services.AddSingleton(options.Options);
            }

            return services;
        }
    }
}
