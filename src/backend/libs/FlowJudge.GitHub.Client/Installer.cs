using FlowJudge.GitHub.Client.Auth;
using FlowJudge.GitHub.Client.Clients;
using FlowJudge.GitHub.Client.Clients.Impl;
using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.GitHub.Client
{
    public static class Installer
    {
        public static IServiceCollection AddGitHubClient(
            this IServiceCollection services,
            Action<IServiceProvider, GitHubClientConfigurationBuilder> configure)
        {
            services.AddSingleton(sp =>
            {
                var configurationBuilder = new GitHubClientConfigurationBuilder();

                configure(sp, configurationBuilder);

                return configurationBuilder.Build();
            });

            services.AddScoped<IGitHubService, ServiceImpl.GitHubService>();
            services.AddScoped<IInstallationAuthService, InstallationAuthService>();

            services.AddHttpClient<InstallationAuthHttpClient>((ctx, client) =>
            {
                var configuration = ctx.GetRequiredService<GitHubClientConfiguration>();
                client.BaseAddress = new Uri(configuration.BaseApiUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
                client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", configuration.ApiVersion);
            });

            services.AddHttpClient<IRepositoryClient, RepositoryClient>((ctx, client) =>
            {
                var configuration = ctx.GetRequiredService<GitHubClientConfiguration>();
                client.BaseAddress = new Uri(configuration.BaseApiUrl);
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/vnd.github+json"));
                client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", configuration.ApiVersion);
            });

            return services;
        }

        /// <summary>
        /// Registeres GitHub client with token store
        /// </summary>
        /// <typeparam name="TTokenStore">Your implementation of ITokenStore</typeparam>
        public static IServiceCollection AddGitHubClient<TTokenStore>(
            this IServiceCollection services,
            Action<IServiceProvider, GitHubClientConfigurationBuilder> configure)
                where TTokenStore : class, ITokenStore
        {
            services.AddGitHubClient(configure);
            services.AddScoped<ITokenStore, TTokenStore>();

            return services;
        }
    }
}
