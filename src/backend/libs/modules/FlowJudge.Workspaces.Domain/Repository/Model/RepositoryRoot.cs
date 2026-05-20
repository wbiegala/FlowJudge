using FlowJudge.Common.Domain;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Domain.Repository.Model
{
    public sealed class RepositoryRoot : AggregateRoot
    {
        public RepositoryId AggregateId { get; private set; }
        public WorkspaceId WorkspaceId { get; private set; }
        public IntegrationId IntegrationId { get; private set; }

        /// <summary>
        /// Repository id in external VCS
        /// </summary>
        public RepositoryExternalId ExternalId { get; private set; }

        public RepositoryName Name { get; private set; }
        public RepositoryFullName? FullName { get; private set; }
        public bool TrackingEnabled { get; private set; }


        public static RepositoryRoot Create(
            Guid workspaceId,
            Guid integrationId,
            string externalId,
            string name,
            string? fullname,
            bool trackingEnabled)
        {
            return new RepositoryRoot
            {
                AggregateId = RepositoryId.New(),
                WorkspaceId = WorkspaceId.Create(workspaceId),
                IntegrationId = IntegrationId.Create(integrationId),
                ExternalId = RepositoryExternalId.Create(externalId),
                Name = RepositoryName.Create(name),
                FullName = string.IsNullOrWhiteSpace(fullname)
                    ? null
                    : RepositoryFullName.Create(fullname),
                TrackingEnabled = trackingEnabled
            };
        }

        public static RepositoryRoot Load(
            Guid id,
            Guid aggregateId,
            Guid workspaceId,
            Guid integrationId,
            string externalId,
            string name,
            string? fullname,
            bool trackingEnabled)
        {
            return new RepositoryRoot
            {
                Id = id,
                AggregateId = RepositoryId.Create(aggregateId),
                WorkspaceId = WorkspaceId.Create(workspaceId),
                IntegrationId = IntegrationId.Create(integrationId),
                ExternalId = RepositoryExternalId.Create(externalId),
                Name = RepositoryName.Create(name),
                FullName = string.IsNullOrWhiteSpace(fullname)
                    ? null
                    : RepositoryFullName.Create(fullname),
                TrackingEnabled = trackingEnabled
            };
        }
    }
}
