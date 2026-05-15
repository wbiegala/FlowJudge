using FlowJudge.Workspaces.Domain.Integration.Model;

namespace FlowJudge.Workspaces.Application.Abstractions.Models
{
    public sealed record IntegrationListItem
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public IntegrationProvider Provider { get; init; }
        public IntegrationStatus Status { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public Guid CreatedBy { get; init; }
    }
}
