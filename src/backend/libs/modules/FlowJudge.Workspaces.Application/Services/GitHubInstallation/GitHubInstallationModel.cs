namespace FlowJudge.Workspaces.Application.Services.GitHubInstallation
{
    internal sealed class GitHubInstallationModel
    {
        public Guid StateId { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid IssuerId { get; set; }
    }
}
