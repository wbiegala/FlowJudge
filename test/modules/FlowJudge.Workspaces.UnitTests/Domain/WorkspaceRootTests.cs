using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model.Exceptions;

namespace FlowJudge.Workspaces.UnitTests.Domain
{
    public class WorkspaceRootTests
    {
        [Fact]
        public void CreateWorkspace_ShouldCreateWorkspaceWithOwner()
        {
            // Act
            var workspace = WorkspaceRoot.Create(WorkspaceName, CreatorId, CreationTimestamp);

            // Assert
            Assert.IsAssignableFrom<WorkspaceRoot>(workspace);
            Assert.NotNull(workspace);
            Assert.True(workspace.Name == WorkspaceName);
            Assert.True(workspace.CreatedAt == CreationTimestamp);
            Assert.True(workspace.CreatedBy == CreatorId);
            Assert.True(workspace.Members.Count == 1);
            Assert.True(workspace.Members.Single().MemberId == CreatorId);
            Assert.True(workspace.Members.Single().AssignedBy is null);
        }

        [Fact]
        public void SetMember_ShouldAddMember()
        {
            // Arrange
            var member = WorkspaceMember.Load(Guid.NewGuid(), WorkspaceAggregateId,
                CreatorId, WorkspaceRole.Owner, CreationTimestamp, null);
            var workspace = WorkspaceRoot.Load(WorkspaceDbId, WorkspaceAggregateId, WorkspaceName,
                WorkspaceStatus.Active, CreationTimestamp, CreatorId, new List<WorkspaceMember> { member });

            // Act
            workspace.AddOrUpdateMember(MemberId, WorkspaceRole.Member, DateTimeOffset.UtcNow, CreatorId);

            // Assert
            Assert.True(workspace.Members.Count == 2);
            Assert.True(workspace.Members.Any(m => m.MemberId == MemberId));
        }

        [Fact]
        public void SetMember_WhenRoleIsToLow()
        {
            // Arrange
            var owner = WorkspaceMember.Load(Guid.NewGuid(), WorkspaceAggregateId,
                CreatorId, WorkspaceRole.Owner, CreationTimestamp, null);
            var member = WorkspaceMember.Load(Guid.NewGuid(), WorkspaceAggregateId,
                MemberId, WorkspaceRole.Member, CreationTimestamp.AddMinutes(5), null);
            var workspace = WorkspaceRoot.Load(WorkspaceDbId, WorkspaceAggregateId, WorkspaceName,
                WorkspaceStatus.Active, CreationTimestamp, CreatorId, new List<WorkspaceMember> { owner, member });

            // Act
            Assert.Throws<ForbiddenActionException>(() => workspace.AddOrUpdateMember(Guid.NewGuid(), WorkspaceRole.Member, DateTimeOffset.UtcNow, MemberId));
        }

        private static Guid WorkspaceDbId = Guid.Parse("a1b2c3d4-e5f6-7890-abcd-ef1234567890");
        private static Guid WorkspaceAggregateId = Guid.Parse("f1e2d3c4-b5a6-7890-abcd-ef0987654321");
        private const string WorkspaceName = "Test workspace";
        private static Guid CreatorId = Guid.Parse("c889b6c0-8abc-43cf-9eb2-b336b03c551f");
        private static DateTimeOffset CreationTimestamp = new DateTimeOffset(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);
        private static Guid MemberId = Guid.Parse("d1234567-89ab-4cde-f012-3456789abcde");
    }
}
