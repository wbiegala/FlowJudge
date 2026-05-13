using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Application.Abstractions.Models
{
    public sealed record WorkspaceData
    {
        public Guid WorkspaceId { get; init; }
        public required string Name { get; init; }
        public WorkspaceStatus Status { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
        public Guid CreatedBy { get; init; }
        public required IEnumerable<WorkspaceMemberData> Members { get; init; }

        public sealed record WorkspaceMemberData
        {
            public Guid MemberId { get; init; }
            public WorkspaceRole Role { get; init; }
            public Guid? AssingedBy { get; init; }
            public DateTimeOffset AssignedAt { get; init; }
        }
    }
}
