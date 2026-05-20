using System.Reflection.Metadata;

namespace FlowJudge.Workspaces.Application.Services.GitHubInstallation.Models
{
    internal sealed class GitHubInstallationModel
    {
        public Guid StateId { get; set; }
        public Guid WorkspaceId { get; set; }
        public Guid IssuerId { get; set; }
        public string? Name { get; set; }
        public string? InstallationId { get; set; }
        public string? SetupAction { get; set; }
        public IEnumerable<GitHubRepository>? Repositories { get; set; }
        
        internal sealed class GitHubRepository
        {
            public int Id { get; set; }
            public required string Name { get; set; }
            public required string FullName { get; set; }
        }
    }
}
