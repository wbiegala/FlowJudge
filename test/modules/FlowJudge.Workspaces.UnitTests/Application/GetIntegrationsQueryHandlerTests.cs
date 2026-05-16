using FlowJudge.Common.Application;
using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Queries;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using Moq;

namespace FlowJudge.Workspaces.UnitTests.Application
{
    public class GetIntegrationsQueryHandlerTests
    {
        [Fact]
        public async Task HandleAsync_WhenIssuerHasPermissions_ThenReturnIntegrations()
        {
            // Arrange
            var pagination = Fixture.CreatePageQuery(PageSize, PageNumber);
            var integration = Fixture.CreateIntegrationListItem(
                IntegrationId,
                IntegrationName,
                IntegrationProvider.GitHub,
                IntegrationStatus.Inactive,
                CreationTimestamp,
                IssuerId);
            var integrations = Fixture.CreateIntegrationPagedList(
                new[] { integration },
                PageSize,
                PageNumber,
                totalCount: 1);

            _workspaceRepositoryMock
                .Setup(wr => wr.GetUserRoleInWorkspaceAsync(
                    It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                    IssuerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(WorkspaceRole.Member);

            _integrationRepositoryMock
                .Setup(ir => ir.GetIntegrationsAsync(
                    It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                    pagination,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(integrations);

            var query = Fixture.GetIntegrationsQuery(WorkspaceId, IssuerId, pagination);
            var handler = CreateHandler();

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Same(integrations, result.Data);
            Assert.Single(result.Data);
            Assert.Equal(integration, result.Data.Single());
            _workspaceRepositoryMock.Verify(wr => wr.GetUserRoleInWorkspaceAsync(
                It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                IssuerId,
                It.IsAny<CancellationToken>()), Times.Once);
            _integrationRepositoryMock.Verify(ir => ir.GetIntegrationsAsync(
                It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                pagination,
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task HandleAsync_WhenIssuerHasInsufficientPermissions_ThenReturnFailure()
        {
            // Arrange
            var pagination = Fixture.CreatePageQuery(PageSize, PageNumber);

            _workspaceRepositoryMock
                .Setup(wr => wr.GetUserRoleInWorkspaceAsync(
                    It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                    IssuerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((WorkspaceRole?)null);

            var query = Fixture.GetIntegrationsQuery(WorkspaceId, IssuerId, pagination);
            var handler = CreateHandler();

            // Act
            var result = await handler.HandleAsync(query);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data);
            Assert.NotNull(result.Error);
            Assert.True(result.Error.IsInsufficientPermissions());
            _workspaceRepositoryMock.Verify(wr => wr.GetUserRoleInWorkspaceAsync(
                It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                IssuerId,
                It.IsAny<CancellationToken>()), Times.Once);
            _integrationRepositoryMock.Verify(ir => ir.GetIntegrationsAsync(
                It.IsAny<WorkspaceId>(),
                It.IsAny<PageQuery>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        private GetIntegrationsQueryHandler CreateHandler() =>
            new(_workspaceRepositoryMock.Object, _integrationRepositoryMock.Object);

        private readonly Mock<IWorkspaceRepository> _workspaceRepositoryMock = new();
        private readonly Mock<IIntegrationRepository> _integrationRepositoryMock = new();

        private static readonly Guid WorkspaceId = Guid.Parse("9b591c5d-5b24-4920-b76b-89410003f522");
        private static readonly Guid IssuerId = Guid.Parse("ff813934-ae55-4fd2-aa71-946038a876b0");
        private static readonly Guid IntegrationId = Guid.Parse("69d55f2b-b957-40e9-9f8d-2ec5e8e9bd57");
        private const string IntegrationName = "GitHub";
        private const int PageSize = 10;
        private const int PageNumber = 1;
        private static readonly DateTimeOffset CreationTimestamp = new(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);
    }
}
