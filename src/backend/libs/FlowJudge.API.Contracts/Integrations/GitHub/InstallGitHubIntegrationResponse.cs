namespace FlowJudge.API.Contracts.Integrations.GitHub
{
    public sealed record InstallGitHubIntegrationResponse
    {
        public required string RedirectUrl { get; init; }
    }
}
