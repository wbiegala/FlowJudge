using FlowJudge.GitHub.Webhooks.Security;
using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.GitHub.Webhooks
{
    public static class Installer
    {
        public static IServiceCollection AddGitHubWebhooks(
            this IServiceCollection services,
            string webhooksSecret)
        {
            services.AddSingleton<IGitHubWebhookSignatureValidator>(_ => new GitHubWebhookSignatureValidator(webhooksSecret));

            return services;
        }
    }
}
