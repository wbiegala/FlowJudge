using FlowJudge.Common.Application;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Workspaces.Application.Abstractions.Commands;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Commands;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Integration.Services;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using Moq;

namespace FlowJudge.Workspaces.UnitTests.Application
{
    public class CreateIntegrationCommandHandlerTests
    {
        [Fact]
        public async Task ExecuteAsync_WhenIssuerHasPermissions_ThenCreateIntegration()
        {
            // Arrange
            var integration = Fixture.CreateGithubIntegration(
                IntegrationDbId,
                IntegrationAggregateId,
                WorkspaceId,
                IntegrationName,
                IntegrationStatus.Inactive,
                CreationTimestamp,
                IssuerId);

            _workspaceRepositoryMock
                .Setup(wr => wr.GetUserRoleInWorkspaceAsync(
                    It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                    IssuerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(WorkspaceRole.Administrator);

            _integrationFactoryMock
                .Setup(f => f.CreateGithubIntegration(
                    It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                    It.Is<IntegrationName>(name => name.Value == IntegrationName),
                    IssuerId))
                .Returns(integration);

            var command = Fixture.CreateIntegrationCommand(
                IntegrationName,
                IntegrationProvider.GitHub,
                WorkspaceId,
                IssuerId);
            var handler = CreateHandler();

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(IntegrationAggregateId, result.Data);
            _workspaceRepositoryMock.Verify(wr => wr.GetUserRoleInWorkspaceAsync(
                It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                IssuerId,
                It.IsAny<CancellationToken>()), Times.Once);
            _integrationFactoryMock.Verify(f => f.CreateGithubIntegration(
                It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                It.Is<IntegrationName>(name => name.Value == IntegrationName),
                IssuerId), Times.Once);
            _integrationRepositoryMock.Verify(ir => ir.AddIntegrationAsync(integration, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenIssuerHasInsufficientPermissions_ThenReturnFailure()
        {
            // Arrange
            _workspaceRepositoryMock
                .Setup(wr => wr.GetUserRoleInWorkspaceAsync(
                    It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                    IssuerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(WorkspaceRole.Member);

            var command = Fixture.CreateIntegrationCommand(
                IntegrationName,
                IntegrationProvider.GitHub,
                WorkspaceId,
                IssuerId);
            var handler = CreateHandler();

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.True(result.Error.IsInsufficientPermissions());
            _workspaceRepositoryMock.Verify(wr => wr.GetUserRoleInWorkspaceAsync(
                It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                IssuerId,
                It.IsAny<CancellationToken>()), Times.Once);
            _integrationFactoryMock.Verify(f => f.CreateGithubIntegration(
                It.IsAny<WorkspaceId>(),
                It.IsAny<IntegrationName>(),
                It.IsAny<Guid>()), Times.Never);
            _integrationRepositoryMock.Verify(ir => ir.AddIntegrationAsync(It.IsAny<IntegrationRoot>(), It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        private CreateIntegrationCommandHandler CreateHandler() =>
            new(
                _workspaceRepositoryMock.Object,
                _integrationRepositoryMock.Object,
                _integrationFactoryMock.Object,
                _unitOfWorkMock.Object);

        private readonly Mock<IWorkspaceRepository> _workspaceRepositoryMock = new();
        private readonly Mock<IIntegrationRepository> _integrationRepositoryMock = new();
        private readonly Mock<IIntegrationFactory> _integrationFactoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private static readonly Guid WorkspaceId = Guid.Parse("9b591c5d-5b24-4920-b76b-89410003f522");
        private static readonly Guid IssuerId = Guid.Parse("ff813934-ae55-4fd2-aa71-946038a876b0");
        private static readonly Guid IntegrationDbId = Guid.Parse("f3983c62-6980-4606-a6f7-02fb782729a1");
        private static readonly Guid IntegrationAggregateId = Guid.Parse("69d55f2b-b957-40e9-9f8d-2ec5e8e9bd57");
        private const string IntegrationName = "GitHub";
        private static readonly DateTimeOffset CreationTimestamp = new(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);
    }
}
