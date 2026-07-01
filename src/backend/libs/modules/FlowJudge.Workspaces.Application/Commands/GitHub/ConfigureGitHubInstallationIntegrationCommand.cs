using FlowJudge.Common.Application.Mediator;
using FlowJudge.Workspaces.Domain.Integration.Model;

namespace FlowJudge.Workspaces.Application.Commands.GitHub
{
    internal sealed record ConfigureGitHubInstallationIntegrationCommand : ICommand<Guid>
    {
        public Guid IntegrationId { get; init; }
        public Guid IssuerId { get; init; }
        public required string Name { get; init; }
        public IntegrationStatus InitialStatus { get; init; }
        public required IEnumerable<GitHubRepositoryInitialConfiguration> Repositories { get; init; }

        internal sealed record GitHubRepositoryInitialConfiguration
        {
            public int GitHubId { get; init; }
            public required string Name { get; init; }
            public required string FullName { get; init; }
            public bool TrackingEnabled { get; init; }
        }
    }
}
