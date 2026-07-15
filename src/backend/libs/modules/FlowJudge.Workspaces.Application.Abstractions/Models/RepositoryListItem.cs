using FlowJudge.Workspaces.Domain.Repository.Model;

namespace FlowJudge.Workspaces.Application.Abstractions.Models
{
    public sealed record RepositoryListItem
    {
        public Guid Id { get; init; }
        public Guid IntegrationId { get; init; }
        public required string Name { get; init; }
        public string? FullName { get; init; }
        public bool TrackingEnabled { get; init; }
        public RepositoryStatus Status { get; init; }
    }
}
