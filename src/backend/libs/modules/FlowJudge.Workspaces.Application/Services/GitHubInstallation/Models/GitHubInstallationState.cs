namespace FlowJudge.Workspaces.Application.Services.GitHubInstallation.Models
{
    internal sealed class GitHubInstallationState
    {
        public Guid StateId { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid IssuerId { get; set; }
        public string? Name { get; set; }
        public string? InstallationId { get; set; }
    }
}
