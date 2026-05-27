namespace FlowJudge.VCS.Contracts.Events.Data
{
    public sealed record IntegrationChangedEventData : IEventData
    {
        public required string IntegrationId { get; init; }
        public required IntegrationAction Action { get; init; }

        public enum IntegrationAction
        {
            Created,
            Deleted,
            PermissionsChanged,
            Deactivated,
            Reactivated
        }
    }
}
