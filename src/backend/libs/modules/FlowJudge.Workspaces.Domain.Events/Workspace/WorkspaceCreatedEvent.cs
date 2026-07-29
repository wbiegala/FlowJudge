using FlowJudge.Common.Domain;
using FlowJudge.Common.Messaging;

namespace FlowJudge.Workspaces.Domain.Events.Workspace
{
    [OutboxSubject("flowjudge.domain.event.workspace-created")]
    public sealed record WorkspaceCreatedEvent : DomainEvent, IMessage
    {
        public Guid MessageId { get; init; } = Guid.NewGuid();
        public Guid WorkspaceId { get; init; }
        public int Status { get; init; }
    }
}
