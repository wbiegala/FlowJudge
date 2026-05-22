using FlowJudge.Common.Secrets;

namespace FlowJudge.API.Service.Installers
{
    public static class SecretsInstaller
    {
        public const string GithubPrivateKeySecretName = "GithubPrivateKey";

        public static void InstallSecrets(this WebApplicationBuilder builder)
        {
            InstallGithubSecrets(builder);
        }

        private static void InstallGithubSecrets(WebApplicationBuilder builder)
        {
            var githubPrivateKeyPath = builder.Configuration["Integrations:GitHub:PrivateKeyPath"];
            if (string.IsNullOrWhiteSpace(githubPrivateKeyPath))
            {
                throw new InvalidOperationException($"GitHub private key path is not configured.");
            }

            builder.Services.AddFileSecretProvider(GithubPrivateKeySecretName, githubPrivateKeyPath);
        }
    }
}
