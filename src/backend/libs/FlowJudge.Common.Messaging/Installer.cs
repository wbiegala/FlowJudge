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
    }
}
