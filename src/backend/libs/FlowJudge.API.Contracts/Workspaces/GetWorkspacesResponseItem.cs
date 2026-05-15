using FlowJudge.API.Contracts.Shared;

namespace FlowJudge.API.Contracts.Workspaces
{
    public sealed record GetWorkspacesResponseItem
    {
        public Guid WorkspaceId { get; init; }
        public required string Name { get; init; }
        public required UserData Owner { get; init; }
        public required string RoleName { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
    }
}
