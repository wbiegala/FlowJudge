using FlowJudge.Common.Application.Mediator;

namespace FlowJudge.Workspaces.Application.Commands.GitHub
{
    internal sealed record CreateGitHubInstallationIntegrationCommand : ICommand<Guid>
    {
        public Guid WorkspaceId { get; init; }
        public Guid IssuerId { get; init; }
        public required string Name { get; init; }
        public required string GitHubInstallationId { get; init; }
    }
}
