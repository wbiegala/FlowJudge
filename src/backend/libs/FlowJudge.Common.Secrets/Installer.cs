using FlowJudge.Common.Secrets.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.Common.Secrets
{
    public static class Installer
    {
        public static IServiceCollection AddFileSecretProvider(
            this IServiceCollection services,
            string secretName,
            string filePath)
        {
            services.AddKeyedSingleton<ISecretProvider>(secretName, (sp, key) => new FileSecretProvider(filePath));

            return services;
        }
    }
}
