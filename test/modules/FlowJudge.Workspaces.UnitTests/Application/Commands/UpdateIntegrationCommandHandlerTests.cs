using FlowJudge.Common.Application;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Workspaces.Application.Abstractions.Commands;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Commands;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Repository.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using Moq;

namespace FlowJudge.Workspaces.UnitTests.Application.Commands
{
    public class UpdateIntegrationCommandHandlerTests
    {
        [Fact]
        public async Task ExecuteAsync_WhenIssuerHasPermissions_ThenUpdateIntegrationAndRepositoryTrackingSettings()
        {
            // Arrange
            var integration = CreateIntegration();
            var repositoryToEnable = Fixture.CreateRepository(
                RepositoryDbId,
                RepositoryAggregateId,
                WorkspaceId,
                IntegrationAggregateId,
                RepositoryExternalId,
                RepositoryName,
                RepositoryFullName,
                trackingEnabled: false);
            var repositoryToDisable = Fixture.CreateRepository(
                OtherRepositoryDbId,
                OtherRepositoryAggregateId,
                WorkspaceId,
                IntegrationAggregateId,
                OtherRepositoryExternalId,
                OtherRepositoryName,
                OtherRepositoryFullName,
                trackingEnabled: true);

            _integrationRepositoryMock
                .Setup(ir => ir.GetIntegrationByAggregateIdAsync(
                    It.Is<IntegrationId>(id => id.Value == IntegrationAggregateId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(integration);

            _workspaceRepositoryMock
                .Setup(wr => wr.GetUserRoleInWorkspaceAsync(
                    It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                    IssuerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(WorkspaceRole.Administrator);

            _repositoryRepositoryMock
                .Setup(rr => rr.GetRepositoriesByIntegrationAsync(
                    It.Is<IntegrationId>(id => id.Value == IntegrationAggregateId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { repositoryToEnable, repositoryToDisable });

            var command = Fixture.UpdateIntegrationCommand(
                WorkspaceId,
                IntegrationAggregateId,
                IssuerId,
                UpdatedIntegrationName,
                IntegrationStatus.Inactive,
                new[]
                {
                    new UpdateIntegrationCommand.RepositoryTrackingSettings
                    {
                        RepositoryId = RepositoryAggregateId,
                        TrackingEnabled = true
                    },
                    new UpdateIntegrationCommand.RepositoryTrackingSettings
                    {
                        RepositoryId = OtherRepositoryAggregateId,
                        TrackingEnabled = false
                    }
                });
            var handler = CreateHandler();

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(UpdatedIntegrationName, integration.Name.Value);
            Assert.Equal(IntegrationStatus.Inactive, integration.Status);
            Assert.True(repositoryToEnable.TrackingEnabled);
            Assert.False(repositoryToDisable.TrackingEnabled);
            _integrationRepositoryMock.Verify(ir => ir.UpdateIntegrationAsync(integration, It.IsAny<CancellationToken>()), Times.Once);
            _repositoryRepositoryMock.Verify(rr => rr.UpdateRepositoryAsync(repositoryToEnable, It.IsAny<CancellationToken>()), Times.Once);
            _repositoryRepositoryMock.Verify(rr => rr.UpdateRepositoryAsync(repositoryToDisable, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenIssuerIsSystem_ThenUpdateIntegrationWithoutPermissionCheck()
        {
            // Arrange
            var integration = CreateIntegration();

            _integrationRepositoryMock
                .Setup(ir => ir.GetIntegrationByAggregateIdAsync(
                    It.Is<IntegrationId>(id => id.Value == IntegrationAggregateId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(integration);

            _repositoryRepositoryMock
                .Setup(rr => rr.GetRepositoriesByIntegrationAsync(
                    It.Is<IntegrationId>(id => id.Value == IntegrationAggregateId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(Enumerable.Empty<RepositoryRoot>());

            var command = Fixture.UpdateIntegrationCommand(
                WorkspaceId,
                IntegrationAggregateId,
                Guid.Empty,
                UpdatedIntegrationName,
                IntegrationStatus.Inactive,
                Array.Empty<UpdateIntegrationCommand.RepositoryTrackingSettings>());
            var handler = CreateHandler();

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(UpdatedIntegrationName, integration.Name.Value);
            Assert.Equal(IntegrationStatus.Inactive, integration.Status);
            _workspaceRepositoryMock.Verify(wr => wr.GetUserRoleInWorkspaceAsync(
                It.IsAny<WorkspaceId>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _integrationRepositoryMock.Verify(ir => ir.UpdateIntegrationAsync(integration, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenIntegrationDoesNotExist_ThenReturnFailure()
        {
            // Arrange
            var command = Fixture.UpdateIntegrationCommand(
                WorkspaceId,
                IntegrationAggregateId,
                IssuerId,
                UpdatedIntegrationName,
                IntegrationStatus.Inactive,
                Array.Empty<UpdateIntegrationCommand.RepositoryTrackingSettings>());
            var handler = CreateHandler();

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.True(result.Error.IsNotFound());
            _workspaceRepositoryMock.Verify(wr => wr.GetUserRoleInWorkspaceAsync(
                It.IsAny<WorkspaceId>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _integrationRepositoryMock.Verify(ir => ir.UpdateIntegrationAsync(
                It.IsAny<IntegrationRoot>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _repositoryRepositoryMock.Verify(rr => rr.GetRepositoriesByIntegrationAsync(
                It.IsAny<IntegrationId>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenIntegrationBelongsToDifferentWorkspace_ThenReturnFailure()
        {
            // Arrange
            var integration = Fixture.CreateGithubIntegration(
                IntegrationDbId,
                IntegrationAggregateId,
                OtherWorkspaceId,
                IntegrationName,
                IntegrationStatus.Active,
                CreationTimestamp,
                IssuerId);

            _integrationRepositoryMock
                .Setup(ir => ir.GetIntegrationByAggregateIdAsync(
                    It.Is<IntegrationId>(id => id.Value == IntegrationAggregateId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(integration);

            var command = Fixture.UpdateIntegrationCommand(
                WorkspaceId,
                IntegrationAggregateId,
                IssuerId,
                UpdatedIntegrationName,
                IntegrationStatus.Inactive,
                Array.Empty<UpdateIntegrationCommand.RepositoryTrackingSettings>());
            var handler = CreateHandler();

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.True(result.Error.IsNotFound());
            _workspaceRepositoryMock.Verify(wr => wr.GetUserRoleInWorkspaceAsync(
                It.IsAny<WorkspaceId>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _integrationRepositoryMock.Verify(ir => ir.UpdateIntegrationAsync(
                It.IsAny<IntegrationRoot>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _repositoryRepositoryMock.Verify(rr => rr.GetRepositoriesByIntegrationAsync(
                It.IsAny<IntegrationId>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenIssuerHasInsufficientPermissions_ThenReturnFailure()
        {
            // Arrange
            var integration = CreateIntegration();

            _integrationRepositoryMock
                .Setup(ir => ir.GetIntegrationByAggregateIdAsync(
                    It.Is<IntegrationId>(id => id.Value == IntegrationAggregateId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(integration);

            _workspaceRepositoryMock
                .Setup(wr => wr.GetUserRoleInWorkspaceAsync(
                    It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                    IssuerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(WorkspaceRole.Member);

            var command = Fixture.UpdateIntegrationCommand(
                WorkspaceId,
                IntegrationAggregateId,
                IssuerId,
                UpdatedIntegrationName,
                IntegrationStatus.Inactive,
                Array.Empty<UpdateIntegrationCommand.RepositoryTrackingSettings>());
            var handler = CreateHandler();

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.True(result.Error.IsInsufficientPermissions());
            _integrationRepositoryMock.Verify(ir => ir.UpdateIntegrationAsync(
                It.IsAny<IntegrationRoot>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _repositoryRepositoryMock.Verify(rr => rr.GetRepositoriesByIntegrationAsync(
                It.IsAny<IntegrationId>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenRepositoryTrackingSettingsDoNotBelongToIntegration_ThenRollback()
        {
            // Arrange
            var integration = CreateIntegration();
            var repository = Fixture.CreateRepository(
                RepositoryDbId,
                RepositoryAggregateId,
                WorkspaceId,
                IntegrationAggregateId,
                RepositoryExternalId,
                RepositoryName,
                RepositoryFullName,
                trackingEnabled: false);

            _integrationRepositoryMock
                .Setup(ir => ir.GetIntegrationByAggregateIdAsync(
                    It.Is<IntegrationId>(id => id.Value == IntegrationAggregateId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(integration);

            _workspaceRepositoryMock
                .Setup(wr => wr.GetUserRoleInWorkspaceAsync(
                    It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                    IssuerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(WorkspaceRole.Administrator);

            _repositoryRepositoryMock
                .Setup(rr => rr.GetRepositoriesByIntegrationAsync(
                    It.Is<IntegrationId>(id => id.Value == IntegrationAggregateId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(new[] { repository });

            var command = Fixture.UpdateIntegrationCommand(
                WorkspaceId,
                IntegrationAggregateId,
                IssuerId,
                UpdatedIntegrationName,
                IntegrationStatus.Inactive,
                new[]
                {
                    new UpdateIntegrationCommand.RepositoryTrackingSettings
                    {
                        RepositoryId = MissingRepositoryAggregateId,
                        TrackingEnabled = true
                    }
                });
            var handler = CreateHandler();

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Equal(nameof(InvalidOperationException), result.Error.Code);
            _repositoryRepositoryMock.Verify(rr => rr.UpdateRepositoryAsync(
                It.IsAny<RepositoryRoot>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        private UpdateIntegrationCommandHandler CreateHandler() =>
            new(
                _workspaceRepositoryMock.Object,
                _integrationRepositoryMock.Object,
                _repositoryRepositoryMock.Object,
                _unitOfWorkMock.Object);

        private static GithubIntegration CreateIntegration() =>
            Fixture.CreateGithubIntegration(
                IntegrationDbId,
                IntegrationAggregateId,
                WorkspaceId,
                IntegrationName,
                IntegrationStatus.Active,
                CreationTimestamp,
                IssuerId);

        private readonly Mock<IWorkspaceRepository> _workspaceRepositoryMock = new();
        private readonly Mock<IIntegrationRepository> _integrationRepositoryMock = new();
        private readonly Mock<IRepositoryRepository> _repositoryRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private static readonly Guid WorkspaceId = Guid.Parse("9b591c5d-5b24-4920-b76b-89410003f522");
        private static readonly Guid OtherWorkspaceId = Guid.Parse("6ec4dc9c-86a6-459c-b91e-daf17be2e96d");
        private static readonly Guid IssuerId = Guid.Parse("ff813934-ae55-4fd2-aa71-946038a876b0");
        private static readonly Guid IntegrationDbId = Guid.Parse("f3983c62-6980-4606-a6f7-02fb782729a1");
        private static readonly Guid IntegrationAggregateId = Guid.Parse("69d55f2b-b957-40e9-9f8d-2ec5e8e9bd57");
        private static readonly Guid RepositoryDbId = Guid.Parse("a853654b-89ff-44de-ae0e-3c97bd22bda9");
        private static readonly Guid RepositoryAggregateId = Guid.Parse("35965492-ab4d-4645-b2c5-f09eedcbf244");
        private static readonly Guid OtherRepositoryDbId = Guid.Parse("4d004a77-86ad-4800-a695-591c1fd7bbcc");
        private static readonly Guid OtherRepositoryAggregateId = Guid.Parse("87686821-6653-4353-b8ef-21e3bd85169a");
        private static readonly Guid MissingRepositoryAggregateId = Guid.Parse("9bcf9c50-efef-4908-866a-e751053505f0");
        private const string IntegrationName = "GitHub";
        private const string UpdatedIntegrationName = "Configured GitHub";
        private const string RepositoryExternalId = "1234";
        private const string RepositoryName = "flowjudge-api";
        private const string RepositoryFullName = "flowjudge/flowjudge-api";
        private const string OtherRepositoryExternalId = "4567";
        private const string OtherRepositoryName = "flowjudge-web";
        private const string OtherRepositoryFullName = "flowjudge/flowjudge-web";
        private static readonly DateTimeOffset CreationTimestamp = new(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);
    }
}
