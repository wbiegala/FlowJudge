using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.GitHub.Client.ServiceImpl
{
    internal sealed class GitHubService : IGitHubService
    {
        private readonly GitHubClientConfiguration _configuration;
        private readonly IServiceProvider _serviceProvider;

        public GitHubService(IServiceProvider serviceProvider)
        {
            _configuration = serviceProvider.GetRequiredService<GitHubClientConfiguration>();
            _serviceProvider = serviceProvider.GetRequiredService<IServiceProvider>();
        }

        public Task<string> GetInstallationUrlAsync(CancellationToken ct = default)
        {
            var url = $"{_configuration.BaseUrl}/apps/{_configuration.ApplicationName}/installations/new";

            return Task.FromResult(url);
        }

        public TClient GetClient<TClient>()
            where TClient : class, IGitHubClient
        {
            var client = _serviceProvider.GetService<TClient>();
            if (client is null)
                throw new InvalidOperationException();

            return client;
        }
    }
}
