using FlowJudge.Common.Application.Mediator;
using FlowJudge.Workspaces.Domain.Integration.Model;

namespace FlowJudge.Workspaces.Application.Abstractions.Commands
{
    public sealed record UpdateIntegrationCommand : ICommand
    {
        public Guid IntegrationId { get; init; }
        public Guid WorkspaceId { get; init; }
        public Guid IssuerId { get; init; }

        public required string Name { get; init; }
        public IntegrationStatus Status { get; init; }
        public IReadOnlyCollection<RepositoryTrackingSettings> TrackingSettings { get; init; } = Array.Empty<RepositoryTrackingSettings>();

        public sealed record RepositoryTrackingSettings
        {
            public Guid RepositoryId { get; init; }
            public bool TrackingEnabled { get; init; }
        }
    }
}
