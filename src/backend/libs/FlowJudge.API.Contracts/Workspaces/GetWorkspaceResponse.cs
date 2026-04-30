namespace FlowJudge.API.Contracts.Workspaces
{
    public sealed record GetWorkspaceResponse
    {
        public Guid Id { get; init; }
        public required string Name { get; init; }
        public required string Status { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public required WorkspaceUserData CreatedBy { get; init; }
        public required IEnumerable<WorkspaceMemberData> Members { get; init; }
    }
}
