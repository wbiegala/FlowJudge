using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Repository.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace FlowJudge.Workspaces.Application.Abstractions.Models
{
    public sealed record IntegrationData
    {
        public Guid IntegrationId { get; init; }
        public Guid WorkspaceId { get; init; }
        public required string Name { get; init; }
        public IntegrationProvider Provider { get; init; }
        public IntegrationStatus Status { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public Guid CreatedBy { get; init; }
        public required IReadOnlyCollection<IntegrationRepositoryListItem> Repositories { get; init; }     

        public sealed record IntegrationRepositoryListItem
        {
            public Guid RepositoryId { get; init; }
            public Guid IntegrationId { get; init; }
            public Guid WorkspaceId { get; init; }
            public required string Name { get; init; }
            public string? FullName { get; init; }
            public bool TrackingEnabled { get; init; }
            public RepositoryStatus Status { get; init; }
        }
    }
}
