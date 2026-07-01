using FlowJudge.Common.Application;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Commands;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Repository.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using Microsoft.Extensions.Logging;
using Moq;

namespace FlowJudge.Workspaces.UnitTests.Application.Commands
{
    public class ConfigureIntegrationRepositoriesCommandHandlerTests
    {
        [Fact]
        public async Task ExecuteAsync_WhenIssuerHasPermissions_ThenConfigureRepositories()
        {
            // Arrange
            var addedRepositories = new List<RepositoryRoot>();
            var integration = CreateIntegration();
            var existingRepository = Fixture.CreateRepository(
                RepositoryDbId,
                RepositoryAggregateId,
                WorkspaceId,
                IntegrationAggregateId,
                ExistingRepositoryExternalId,
                "old-name",
                "old/full-name",
                trackingEnabled: false);
            var removedRepository = Fixture.CreateRepository(
                RemovedRepositoryDbId,
                RemovedRepositoryAggregateId,
                WorkspaceId,
                IntegrationAggregateId,
                RemovedRepositoryExternalId,
                "removed-repository",
                "flowjudge/removed-repository",
                trackingEnabled: true);
            var newRepositoryConfiguration = Fixture.CreateRepositoryConfiguration(
                WorkspaceId,
                IntegrationAggregateId,
                NewRepositoryExternalId,
                NewRepositoryName,
                NewRepositoryFullName,
                trackingEnabled: true);
            var existingRepositoryConfiguration = Fixture.CreateRepositoryConfiguration(
                WorkspaceId,
                IntegrationAggregateId,
                ExistingRepositoryExternalId,
                ExistingRepositoryName,
                ExistingRepositoryFullName,
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
                .ReturnsAsync(new[] { existingRepository, removedRepository });

            _repositoryRepositoryMock
                .Setup(rr => rr.AddRepositoryAsync(It.IsAny<RepositoryRoot>(), It.IsAny<CancellationToken>()))
                .Callback<RepositoryRoot, CancellationToken>((repository, _) => addedRepositories.Add(repository))
                .Returns(Task.CompletedTask);

            var command = Fixture.ConfigureIntegrationRepositoriesCommand(
                WorkspaceId,
                IntegrationAggregateId,
                IssuerId,
                new[] { newRepositoryConfiguration, existingRepositoryConfiguration });
            var handler = CreateHandler();

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.True(result.IsSuccess);
            var addedRepository = Assert.Single(addedRepositories);
            Assert.Equal(WorkspaceId, addedRepository.WorkspaceId.Value);
            Assert.Equal(IntegrationAggregateId, addedRepository.IntegrationId.Value);
            Assert.Equal(NewRepositoryExternalId, addedRepository.ExternalId.Value);
            Assert.Equal(NewRepositoryName, addedRepository.Name.Value);
            Assert.Equal(NewRepositoryFullName, addedRepository.FullName!.Value);
            Assert.True(addedRepository.TrackingEnabled);
            Assert.Equal(RepositoryStatus.Active, addedRepository.Status);
            Assert.Equal(ExistingRepositoryName, existingRepository.Name.Value);
            Assert.Equal(ExistingRepositoryFullName, existingRepository.FullName!.Value);
            Assert.True(existingRepository.TrackingEnabled);
            Assert.Equal(RepositoryStatus.Deleted, removedRepository.Status);
            _repositoryRepositoryMock.Verify(rr => rr.AddRepositoryAsync(
                It.IsAny<RepositoryRoot>(),
                It.IsAny<CancellationToken>()), Times.Once);
            _repositoryRepositoryMock.Verify(rr => rr.UpdateRepositoryAsync(existingRepository, It.IsAny<CancellationToken>()), Times.Once);
            _repositoryRepositoryMock.Verify(rr => rr.UpdateRepositoryAsync(removedRepository, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenIssuerIsSystem_ThenConfigureRepositoriesWithoutPermissionCheck()
        {
            // Arrange
            var integration = CreateIntegration();
            var repositoryConfiguration = Fixture.CreateRepositoryConfiguration(
                WorkspaceId,
                IntegrationAggregateId,
                NewRepositoryExternalId,
                NewRepositoryName,
                NewRepositoryFullName,
                trackingEnabled: true);

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

            var command = Fixture.ConfigureIntegrationRepositoriesCommand(
                WorkspaceId,
                IntegrationAggregateId,
                Guid.Empty,
                new[] { repositoryConfiguration });
            var handler = CreateHandler();

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.True(result.IsSuccess);
            _workspaceRepositoryMock.Verify(wr => wr.GetUserRoleInWorkspaceAsync(
                It.IsAny<WorkspaceId>(),
                It.IsAny<Guid>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _repositoryRepositoryMock.Verify(rr => rr.AddRepositoryAsync(
                It.IsAny<RepositoryRoot>(),
                It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenIntegrationDoesNotExist_ThenReturnFailure()
        {
            // Arrange
            var command = Fixture.ConfigureIntegrationRepositoriesCommand(
                WorkspaceId,
                IntegrationAggregateId,
                IssuerId,
                Array.Empty<RepositoryConfiguration>());
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

            var command = Fixture.ConfigureIntegrationRepositoriesCommand(
                WorkspaceId,
                IntegrationAggregateId,
                IssuerId,
                Array.Empty<RepositoryConfiguration>());
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

            var command = Fixture.ConfigureIntegrationRepositoriesCommand(
                WorkspaceId,
                IntegrationAggregateId,
                IssuerId,
                Array.Empty<RepositoryConfiguration>());
            var handler = CreateHandler();

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.True(result.Error.IsInsufficientPermissions());
            _repositoryRepositoryMock.Verify(rr => rr.GetRepositoriesByIntegrationAsync(
                It.IsAny<IntegrationId>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _repositoryRepositoryMock.Verify(rr => rr.AddRepositoryAsync(
                It.IsAny<RepositoryRoot>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenRepositoryConfigurationDoesNotBelongToIntegration_ThenReturnFailure()
        {
            // Arrange
            var integration = CreateIntegration();
            var repositoryConfiguration = Fixture.CreateRepositoryConfiguration(
                WorkspaceId,
                OtherIntegrationAggregateId,
                NewRepositoryExternalId,
                NewRepositoryName,
                NewRepositoryFullName,
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

            var command = Fixture.ConfigureIntegrationRepositoriesCommand(
                WorkspaceId,
                IntegrationAggregateId,
                IssuerId,
                new[] { repositoryConfiguration });
            var handler = CreateHandler();

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.True(result.Error.IsInsufficientPermissions());
            _repositoryRepositoryMock.Verify(rr => rr.GetRepositoriesByIntegrationAsync(
                It.IsAny<IntegrationId>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _repositoryRepositoryMock.Verify(rr => rr.AddRepositoryAsync(
                It.IsAny<RepositoryRoot>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenRepositoryConfigurationDoesNotBelongToWorkspace_ThenReturnFailure()
        {
            // Arrange
            var integration = CreateIntegration();
            var repositoryConfiguration = Fixture.CreateRepositoryConfiguration(
                OtherWorkspaceId,
                IntegrationAggregateId,
                NewRepositoryExternalId,
                NewRepositoryName,
                NewRepositoryFullName,
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

            var command = Fixture.ConfigureIntegrationRepositoriesCommand(
                WorkspaceId,
                IntegrationAggregateId,
                IssuerId,
                new[] { repositoryConfiguration });
            var handler = CreateHandler();

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.True(result.Error.IsInsufficientPermissions());
            _repositoryRepositoryMock.Verify(rr => rr.GetRepositoriesByIntegrationAsync(
                It.IsAny<IntegrationId>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _repositoryRepositoryMock.Verify(rr => rr.AddRepositoryAsync(
                It.IsAny<RepositoryRoot>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        private ConfigureIntegrationRepositoriesCommandHandler CreateHandler() =>
            new(
                _workspaceRepositoryMock.Object,
                _integrationRepositoryMock.Object,
                _repositoryRepositoryMock.Object,
                _loggerMock.Object,
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
        private readonly Mock<ILogger<ConfigureIntegrationRepositoriesCommandHandler>> _loggerMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private static readonly Guid WorkspaceId = Guid.Parse("9b591c5d-5b24-4920-b76b-89410003f522");
        private static readonly Guid OtherWorkspaceId = Guid.Parse("6ec4dc9c-86a6-459c-b91e-daf17be2e96d");
        private static readonly Guid IssuerId = Guid.Parse("ff813934-ae55-4fd2-aa71-946038a876b0");
        private static readonly Guid IntegrationDbId = Guid.Parse("f3983c62-6980-4606-a6f7-02fb782729a1");
        private static readonly Guid IntegrationAggregateId = Guid.Parse("69d55f2b-b957-40e9-9f8d-2ec5e8e9bd57");
        private static readonly Guid OtherIntegrationAggregateId = Guid.Parse("8d5e86e6-286b-4793-a893-545075e075d2");
        private static readonly Guid RepositoryDbId = Guid.Parse("a853654b-89ff-44de-ae0e-3c97bd22bda9");
        private static readonly Guid RepositoryAggregateId = Guid.Parse("35965492-ab4d-4645-b2c5-f09eedcbf244");
        private static readonly Guid RemovedRepositoryDbId = Guid.Parse("4d004a77-86ad-4800-a695-591c1fd7bbcc");
        private static readonly Guid RemovedRepositoryAggregateId = Guid.Parse("87686821-6653-4353-b8ef-21e3bd85169a");
        private const string IntegrationName = "GitHub";
        private const string NewRepositoryExternalId = "9876";
        private const string NewRepositoryName = "flowjudge";
        private const string NewRepositoryFullName = "flowjudge/flowjudge";
        private const string ExistingRepositoryExternalId = "1234";
        private const string ExistingRepositoryName = "flowjudge-api";
        private const string ExistingRepositoryFullName = "flowjudge/flowjudge-api";
        private const string RemovedRepositoryExternalId = "4567";
        private static readonly DateTimeOffset CreationTimestamp = new(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);
    }
}
