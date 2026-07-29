using FlowJudge.Common.Domain;
using FlowJudge.Common.Messaging;

namespace FlowJudge.Workspaces.Domain.Events.Workspace
{
    [OutboxSubject("flowjudge.domain.event.workspace-status-changed")]
    public sealed record WorkspaceStatusChangedEvent : DomainEvent, IMessage
    {
        public Guid MessageId { get; init; } = Guid.NewGuid();
        public Guid WorkspaceId { get; init; }
        public int PreviousStatus { get; init; }
        public int CurrentStatus { get; init; }
    }
}
