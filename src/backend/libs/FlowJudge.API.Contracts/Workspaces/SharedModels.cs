using FlowJudge.API.Contracts.Shared;

namespace FlowJudge.API.Contracts.Workspaces
{
    public sealed record WorkspaceMemberData
    {
        public required UserData Member { get; init; }
        public required string Role { get; init; }
        public required UserData? AssignedBy { get; init; }
        public required DateTimeOffset AssingedAt { get; init; }
    }
}
