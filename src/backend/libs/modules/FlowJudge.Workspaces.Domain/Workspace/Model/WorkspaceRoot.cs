using FlowJudge.Common.Domain;
using FlowJudge.Workspaces.Domain.Workspace.Model.Exceptions;

namespace FlowJudge.Workspaces.Domain.Workspace.Model
{
    public sealed class WorkspaceRoot : AggregateRoot
    {
        private readonly List<WorkspaceMember> _members = new();

        /// <summary>
        /// Id aggregate, for use in all external references to this aggregate.
        /// </summary>
        public WorkspaceId AggregateId { get; private set; }

        /// <summary>
        /// Workspace name
        /// </summary>
        public WorkspaceName Name { get; private set; }

        /// <summary>
        /// Workspace status
        /// </summary>
        public WorkspaceStatus Status { get; private set; }

        public DateTimeOffset CreatedAt { get; private set; }

        public Guid CreatedBy { get; private set; }

        /// <summary>
        /// Represents the members of the workspace.
        /// </summary>
        public IReadOnlyCollection<WorkspaceMember> Members => _members.AsReadOnly();

        /// <summary>
        /// Renames the workspace by assigning a new name.
        /// </summary>
        /// <param name="name">The new name to assign to the workspace. Cannot be null.</param>
        public void Rename(WorkspaceName name, Guid issuedBy)
        {
            CheckPermissions(issuedBy, WorkspaceRole.Owner);
            CheckStatus();
            Name = name;
        }

        /// <summary>
        /// Adds a new member to the workspace or updates the role and assignment information for an existing member.
        /// </summary>
        /// <remarks>If the specified user is already a member of the workspace, their role and assignment
        /// details are updated. Otherwise, a new member entry is created.</remarks>
        /// <param name="userId">The unique identifier of the user to add or update as a workspace member.</param>
        /// <param name="role">The role to assign to the user within the workspace.</param>
        /// <param name="timestamp">The date and time when the membership assignment occurs.</param>
        /// <param name="assignedBy">The unique identifier of the user who assigns the role.</param>
        public void AddOrUpdateMember(Guid userId, WorkspaceRole role, DateTimeOffset timestamp, Guid issuedBy)
        {
            CheckPermissions(issuedBy, WorkspaceRole.Administrator);
            CheckStatus();

            if (_members.SingleOrDefault(m => m.MemberId == userId) is WorkspaceMember existingMember)
            {
                if (existingMember.Role == WorkspaceRole.Owner)
                {
                    throw new CannotRemoveOwnershipException();
                }

                _members.Remove(existingMember);
            }

            var member = WorkspaceMember.Create(Id, userId, role, timestamp, issuedBy);
            _members.Add(member);
        }

        /// <summary>
        /// Removes the member with the specified user identifier from the workspace.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to remove from the workspace.</param>
        /// <exception cref="UserIsNotMemberException">Thrown if the specified user is not a member of the workspace.</exception>
        public void RemoveMember(Guid userId, Guid issuedBy)
        {
            CheckPermissions(issuedBy, WorkspaceRole.Administrator);
            CheckStatus();
            if (_members.SingleOrDefault(m => m.MemberId == userId) is WorkspaceMember existingMember)
            {
                if (existingMember.Role == WorkspaceRole.Owner)
                {
                    throw new CannotRemoveOwnershipException();
                }

                _members.Remove(existingMember);
            }
            else
            {
                throw new UserIsNotMemberException();
            }
        }

        public void Archive(Guid issuedBy)
        {
            CheckPermissions(issuedBy, WorkspaceRole.Owner);
            CheckStatus();
            throw new NotImplementedException();
        }

        public static WorkspaceRoot Create(string name, Guid creatorId, DateTimeOffset timestamp)
        {
            var workspace = new WorkspaceRoot
            {
                AggregateId = WorkspaceId.Create(Guid.NewGuid()),
                Name = WorkspaceName.Create(name),
                Status = WorkspaceStatus.Active,
                CreatedAt = timestamp,
                CreatedBy = creatorId
            };

            var ownership = WorkspaceMember.CreateOwnership(workspace.Id, creatorId, timestamp);
            workspace._members.Add(ownership);

            return workspace;
        }

        public static WorkspaceRoot Load(
            Guid id,
            Guid workspaceId,
            string name,
            WorkspaceStatus status,
            DateTimeOffset createdAt,
            Guid createdBy,
            IEnumerable<WorkspaceMember> members)
        {
            var workspace = new WorkspaceRoot
            {
                Id = id,
                AggregateId = WorkspaceId.Create(workspaceId),
                Name = WorkspaceName.Create(name),
                Status = status,
                CreatedAt = createdAt,
                CreatedBy = createdBy
            };

            foreach (var member in members)
            {
                workspace._members.Add(member);
            }

            return workspace;
        }

        private void CheckPermissions(Guid issuerId, WorkspaceRole requiredRole)
        {
            var member = _members.SingleOrDefault(m => m.MemberId == issuerId);
            if (member == null)
                throw new ForbiddenActionException();

            if (member.Role < requiredRole) { 
                throw new ForbiddenActionException();
            }
        }

        private void CheckStatus()
        {
            if (Status != WorkspaceStatus.Active)
            {
                throw new InvalidWorkspaceStatusException();
            }
        }
    }
}
