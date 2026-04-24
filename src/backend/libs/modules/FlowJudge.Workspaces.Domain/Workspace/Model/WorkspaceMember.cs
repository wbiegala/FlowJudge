using FlowJudge.Common.Domain;

namespace FlowJudge.Workspaces.Domain.Workspace.Model
{
    public sealed class WorkspaceMember : Entity
    {
        public Guid WorkspaceId { get; private set; }
        public Guid MemberId { get; private set; }
        public WorkspaceRole Role { get; private set; }
        public DateTimeOffset AssignedAt { get; private set; }
        public Guid? AssignedBy { get; private set; }


        internal static WorkspaceMember CreateOwnership(
            Guid workspaceId,
            Guid ownerId,
            DateTimeOffset timestamp) =>
            new WorkspaceMember
            {
                WorkspaceId = workspaceId,
                MemberId = ownerId,
                Role = WorkspaceRole.Owner,
                AssignedAt = timestamp,
                AssignedBy = null
            };

        internal static WorkspaceMember Create(
            Guid workspaceId,
            Guid memberId,
            WorkspaceRole role,
            DateTimeOffset timestamp,
            Guid? assignedBy) =>
            new WorkspaceMember
            {
                WorkspaceId = workspaceId,
                MemberId = memberId,
                Role = role,
                AssignedAt = timestamp,
                AssignedBy = assignedBy
            };

        public static WorkspaceMember Load(
            Guid id,
            Guid workspaceId,
            Guid memberId,
            WorkspaceRole role,
            DateTimeOffset assignedAt,
            Guid? assignedBy) =>
            new WorkspaceMember
            {
                Id = id,
                WorkspaceId = workspaceId,
                MemberId = memberId,
                Role = role,
                AssignedAt = assignedAt,
                AssignedBy = assignedBy
            };
    }
}