namespace FlowJudge.Workspaces.Application.Services.GitHub
{
    internal sealed class GitHubInstallationModel
    {
        public Guid StateId { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid IssuerId { get; set; }
    }
}
