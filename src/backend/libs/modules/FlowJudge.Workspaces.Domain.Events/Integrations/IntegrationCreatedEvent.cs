using FlowJudge.Common.Domain;
using FlowJudge.Common.Messaging;

namespace FlowJudge.Workspaces.Domain.Events.Integrations
{
    [OutboxSubject("flowjudge.domain.event.integration-created")]
    public sealed record IntegrationCreatedEvent : DomainEvent, IMessage
    {
        public Guid MessageId { get; init; } = Guid.NewGuid();
        public Guid WorkspaceId { get; init; }
        public Guid IntegrationId { get; init; }       
    }
}
