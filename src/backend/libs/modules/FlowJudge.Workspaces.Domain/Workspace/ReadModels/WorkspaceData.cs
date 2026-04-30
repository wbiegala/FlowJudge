using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Domain.Workspace.ReadModels
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

    public static class WorkspaceDataExtensions
    {
        public static WorkspaceData ToReadModel(this WorkspaceRoot workspace)
        {
            return new WorkspaceData
            {
                WorkspaceId = workspace.AggregateId,
                Name = workspace.Name.Value,
                Status = workspace.Status,
                CreatedAt = workspace.CreatedAt,
                CreatedBy = workspace.CreatedBy,
                Members = workspace.Members.Select(m => new WorkspaceData.WorkspaceMemberData
                {
                    MemberId = m.MemberId,
                    Role = m.Role,
                    AssingedBy = m.AssignedBy,
                    AssignedAt = m.AssignedAt
                }).ToList()
            };
        }
    }
}
