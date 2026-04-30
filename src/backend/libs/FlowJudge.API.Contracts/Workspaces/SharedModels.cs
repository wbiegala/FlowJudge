namespace FlowJudge.API.Contracts.Workspaces
{
    public record WorkspaceUserData
    {
        public Guid UserId { get; init; }
        public required string UserName { get; init; }
        public required string EmailAddress { get; init; }
    }

    public sealed record WorkspaceMemberData
    {
        public required WorkspaceUserData Member { get; init; }
        public required string Role { get; init; }
        public required WorkspaceUserData? AssignedBy { get; init; }
        public required DateTimeOffset AssingedAt { get; init; }
    }
}
