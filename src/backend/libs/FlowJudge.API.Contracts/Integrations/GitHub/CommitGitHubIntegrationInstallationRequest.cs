namespace FlowJudge.API.Contracts.Integrations.GitHub
{
    public sealed record CommitGitHubIntegrationInstallationRequest
    {
        public required string Name { get; init; }      
        public required IEnumerable<GitHubRepositoryConfiguration> RepositoriesConfiguration { get; init; } 
    }

    public sealed record GitHubRepositoryConfiguration
    {
        public int GithubId { get; init; }
        public bool Track { get; init; }
    }
}
