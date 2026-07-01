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
        public RepositoryStatus Status { get; private set; }


        public void ChangeName(string repositoryName)
        {
            if (Name != repositoryName)
                Name = RepositoryName.Create(repositoryName);
        }

        public void ChangeFullName(string? repositoryFullName)
        {
            if (string.IsNullOrWhiteSpace(repositoryFullName))
                FullName = null;
            else if (FullName is null || FullName != repositoryFullName)
                FullName = RepositoryFullName.Create(repositoryFullName);
        }

        public void EnableTracking()
        {
            if (TrackingEnabled)
                return;

            TrackingEnabled = true;
        }

        public void DisableTracking()
        {
            if (!TrackingEnabled)
                return;

            TrackingEnabled = false;
        }

        public void Deactivate()
        {
            Status = RepositoryStatus.Deleted;
        }

        public void Reactivate()
        {
            Status = RepositoryStatus.Active;
        }

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
                TrackingEnabled = trackingEnabled,
                Status = RepositoryStatus.Active
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
            bool trackingEnabled,
            string status)
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
                TrackingEnabled = trackingEnabled,
                Status = Enum.Parse<RepositoryStatus>(status)
            };
        }
    }
}
