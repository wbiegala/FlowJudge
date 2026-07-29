using FlowJudge.Common.Domain;
using FlowJudge.Common.Messaging;

namespace FlowJudge.Workspaces.Domain.Events.Repositories
{
    [OutboxSubject("flowjudge.domain.event.repository-status-changed")]
    public sealed record RepositoryStatusChangedEvent : DomainEvent, IMessage
    {
        public Guid MessageId { get; init; } = Guid.NewGuid();
        public Guid RepositoryId { get; init; }
        public Guid IntegrationId {  get; init; }
        public Guid WorkspaceId {  get; init; }
        public int PreviousStatus { get; init; }
        public int CurrentStatus { get; init; }
        public bool IsTrackingEnabled { get; init; }
    }
}
