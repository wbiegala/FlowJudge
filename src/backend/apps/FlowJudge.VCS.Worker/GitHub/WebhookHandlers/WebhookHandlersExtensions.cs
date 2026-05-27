using FlowJudge.VCS.Worker.GitHub.WebhookHandlers.Impl;
using FlowJudge.VCS.Worker.GitHub.WebhooksServices;
using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.VCS.Worker.GitHub.WebhookHandlers
{
    internal static class WebhookHandlersExtensions
    {
        public static IServiceCollection AddGitHubWebhookHandlers(this IServiceCollection services)
        {
            services.AddSingleton<IGitHubWebhookHandler, InstallationWebhookHandler>();

            return services;
        }
    }
}
