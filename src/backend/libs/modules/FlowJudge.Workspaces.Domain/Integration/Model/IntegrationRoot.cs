using FlowJudge.Common.Domain;
using FlowJudge.Workspaces.Domain.Integration.Model.Exceptions;
using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Domain.Integration.Model
{
    public abstract class IntegrationRoot : AggregateRoot
    {
        protected readonly List<IntegrationAuthentication> _authenticationData = new();

        protected IntegrationRoot() { }
        protected IntegrationRoot(
            WorkspaceId workspaceId,
            IntegrationName name,
            IntegrationProvider provider,
            DateTimeOffset createdAt,
            Guid createdBy)
        {
            AggregateId = IntegrationId.Create(Guid.NewGuid());
            WorkspaceId = workspaceId;
            Name = name;
            Provider = provider;
            Status = IntegrationStatus.Inactive;
            CreatedAt = createdAt;
            CreatedBy = createdBy;
        }

        public IntegrationId AggregateId { get; protected set; }
        public WorkspaceId WorkspaceId { get; protected set; }
        public IntegrationName Name { get; protected set; }
        public IntegrationProvider Provider { get; protected set; }
        public IntegrationStatus Status { get; protected set; }
        public IReadOnlyCollection<IntegrationAuthentication> AuthenticationData => _authenticationData.AsReadOnly();
        public DateTimeOffset CreatedAt { get; protected set; }
        public Guid CreatedBy { get; protected set; }


        public virtual void Activate()
        {
            if (Status != IntegrationStatus.Inactive)
                throw new InvalidIntegrationStatusException();

            Status = IntegrationStatus.Active;
        }

        public virtual void Deactivate()
        {
            if (Status != IntegrationStatus.Active)
                throw new InvalidIntegrationStatusException();

            Status = IntegrationStatus.Inactive;
        }

        public void Rename(IntegrationName name)
        {
            Name = name;
        }
    }
}
