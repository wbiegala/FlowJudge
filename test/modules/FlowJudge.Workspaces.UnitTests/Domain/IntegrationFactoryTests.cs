using FlowJudge.Common.Utils.Time;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Integration.Services.Impl;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using Moq;

namespace FlowJudge.Workspaces.UnitTests.Domain
{
    public class IntegrationFactoryTests
    {
        [Fact]
        public void CreateGithubIntegration_ShouldCreateGithubIntegration()
        {
            // Arrange
            _timeServiceMock.Setup(ts => ts.Now).Returns(CreationTimestamp);

            var workspaceId = WorkspaceId.Create(WorkspaceAggregateId);
            var integrationName = IntegrationName.Create(IntegrationDisplayName);
            var factory = new IntegrationFactory(_timeServiceMock.Object);

            // Act
            var integration = factory.CreateGithubIntegration(workspaceId, integrationName, CreatorId);

            // Assert
            Assert.IsAssignableFrom<GithubIntegration>(integration);
            Assert.NotNull(integration);
            Assert.NotEqual(Guid.Empty, integration.AggregateId.Value);
            Assert.Equal(WorkspaceAggregateId, integration.WorkspaceId.Value);
            Assert.Equal(IntegrationDisplayName, integration.Name.Value);
            Assert.Equal(IntegrationProvider.GitHub, integration.Provider);
            Assert.Equal(IntegrationStatus.Inactive, integration.Status);
            Assert.Empty(integration.AuthenticationData);
            Assert.Equal(CreationTimestamp, integration.CreatedAt);
            Assert.Equal(CreatorId, integration.CreatedBy);
            _timeServiceMock.Verify(ts => ts.Now, Times.Once);
        }

        private readonly Mock<ITimeService> _timeServiceMock = new();

        private static readonly Guid WorkspaceAggregateId = Guid.Parse("9b591c5d-5b24-4920-b76b-89410003f522");
        private static readonly Guid CreatorId = Guid.Parse("ff813934-ae55-4fd2-aa71-946038a876b0");
        private const string IntegrationDisplayName = "GitHub";
        private static readonly DateTimeOffset CreationTimestamp = new(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);
    }
}
