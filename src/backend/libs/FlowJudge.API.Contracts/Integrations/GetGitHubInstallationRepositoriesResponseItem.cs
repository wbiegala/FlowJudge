namespace FlowJudge.API.Contracts.Integrations
{
    public sealed record GetGitHubInstallationRepositoriesResponseItem
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string FullName { get; set; }
    }
}
