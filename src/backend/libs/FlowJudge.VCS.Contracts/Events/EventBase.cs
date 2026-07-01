using FlowJudge.Common.Messaging;

namespace FlowJudge.VCS.Contracts.Events
{
    public abstract record EventBase : IMessage
    {
        public Guid MessageId { get; init; }
        public EventType EventType { get; init; }
    }
}
