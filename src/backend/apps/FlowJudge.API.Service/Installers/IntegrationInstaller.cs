using FlowJudge.API.Service.Controllers.Redirects;
using FlowJudge.Common.Secrets;
using FlowJudge.GitHub.Client;

namespace FlowJudge.API.Service.Installers
{
    public static class IntegrationInstaller
    {
        public static void InstallIntegrations(this WebApplicationBuilder builder)
        {
            InstallGitHubIntegration(builder);
        }

        private static void InstallGitHubIntegration(this WebApplicationBuilder builder)
        {
            builder.Services.AddSingleton(ctx =>
            {
                var uiBaseUrl = builder.Configuration["UiRedirectBaseUrl"] ?? string.Empty;
                return new GitHubIntegrationRedirectionService(uiBaseUrl);
            });

            builder.Services.AddGitHubClient((sp, cfg) =>
            {
                var secretProvider = sp.GetRequiredKeyedService<ISecretProvider>(SecretsInstaller.GithubPrivateKeySecretName);
                var githubPrivateKey = secretProvider.GetSecretAsync().GetAwaiter().GetResult();

                if (string.IsNullOrWhiteSpace(githubPrivateKey))
                    throw new InvalidOperationException("GitHub private key is not configured.");

                var applicationId = builder.Configuration["Integrations:GitHub:AppId"];
                if (string.IsNullOrWhiteSpace(applicationId))
                    throw new InvalidOperationException("GitHub App ID is not configured.");

                cfg.WithInstallationTokenAuth(
                    applicationId: applicationId,
                    privateKey: githubPrivateKey);
            });
        }
    }
}
