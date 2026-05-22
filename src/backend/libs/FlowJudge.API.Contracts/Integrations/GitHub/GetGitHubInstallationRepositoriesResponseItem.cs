namespace FlowJudge.API.Contracts.Integrations.GitHub
{
    public sealed record GetGitHubInstallationRepositoriesResponseItem
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string FullName { get; set; }
    }
}
