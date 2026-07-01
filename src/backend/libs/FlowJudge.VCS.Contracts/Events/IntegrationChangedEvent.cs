using FlowJudge.VCS.Contracts.Shared;

namespace FlowJudge.VCS.Contracts.Events
{
    public sealed record IntegrationChangedEvent : EventBase
    {
        public IntegrationProvider Provider { get; init; }
        public required IntegrationData Integration { get; init; }
        public IntegrationAction Action { get; init; }

        public sealed record IntegrationData
        {
            public required string IntegrationId { get; init; }
        }

        public enum IntegrationAction
        {
            Created,
            Updated,
            Deleted,
            Deactivated,
            Reactivated
        }
    }
}
    