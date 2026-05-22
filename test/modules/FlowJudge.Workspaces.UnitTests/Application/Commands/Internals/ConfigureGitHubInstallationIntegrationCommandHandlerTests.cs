using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Common.Application;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Commands.Internals;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Repository.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using Moq;

namespace FlowJudge.Workspaces.UnitTests.Application.Commands.Internals
{
    public class ConfigureGitHubInstallationIntegrationCommandHandlerTests
    {
        [Fact]
        public async Task ExecuteAsync_WhenIssuerHasPermissions_ThenConfigureIntegrationAndRepositories()
        {
            // Arrange
            var addedRepositories = new List<RepositoryRoot>();
            var integration = Fixture.CreateGithubIntegration(
                IntegrationDbId,
                IntegrationAggregateId,
                WorkspaceId,
                "GitHub",
                IntegrationStatus.Inactive,
                CreationTimestamp,
                IssuerId);
            integration.UseInstallation(IntegrationAuthenticationValue.Create(GitHubInstallationId), CreationTimestamp, IssuerId);
            var repositoryConfiguration = Fixture.CreateGitHubRepositoryInitialConfiguration(
                RepositoryGitHubId,
                RepositoryName,
                RepositoryFullName,
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
                .Setup(rr => rr.AddRepositoryAsync(It.IsAny<RepositoryRoot>(), It.IsAny<CancellationToken>()))
                .Callback<RepositoryRoot, CancellationToken>((repository, _) => addedRepositories.Add(repository))
                .Returns(Task.CompletedTask);

            var command = Fixture.ConfigureGitHubInstallationIntegrationCommand(
                IntegrationAggregateId,
                IssuerId,
                IntegrationName,
                IntegrationStatus.Active,
                new[] { repositoryConfiguration });
            var handler = CreateHandler();

            // Act
            var result = await handler.ExecuteAsync(command);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(IntegrationAggregateId, result.Data);
            Assert.Equal(IntegrationName, integration.Name.Value);
            Assert.Equal(IntegrationStatus.Active, integration.Status);
            var repository = Assert.Single(addedRepositories);
            Assert.Equal(WorkspaceId, repository.WorkspaceId.Value);
            Assert.Equal(IntegrationAggregateId, repository.IntegrationId.Value);
            Assert.Equal(RepositoryGitHubId.ToString(), repository.ExternalId.Value);
            Assert.Equal(RepositoryName, repository.Name.Value);
            Assert.Equal(RepositoryFullName, repository.FullName!.Value);
            Assert.True(repository.TrackingEnabled);
            _integrationRepositoryMock.Verify(ir => ir.UpdateIntegrationAsync(integration, It.IsAny<CancellationToken>()), Times.Once);
            _repositoryRepositoryMock.Verify(rr => rr.AddRepositoryAsync(
                It.IsAny<RepositoryRoot>(),
                It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ExecuteAsync_WhenIntegrationDoesNotExist_ThenReturnFailure()
        {
            // Arrange
            var command = Fixture.ConfigureGitHubInstallationIntegrationCommand(
                IntegrationAggregateId,
                IssuerId,
                IntegrationName,
                IntegrationStatus.Active,
                Enumerable.Empty<ConfigureGitHubInstallationIntegrationCommand.GitHubRepositoryInitialConfiguration>());
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
            _repositoryRepositoryMock.Verify(rr => rr.AddRepositoryAsync(
                It.IsAny<RepositoryRoot>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        private ConfigureGitHubInstallationIntegrationCommandHandler CreateHandler() =>
            new(
                _workspaceRepositoryMock.Object,
                _integrationRepositoryMock.Object,
                _repositoryRepositoryMock.Object,
                _unitOfWorkMock.Object);

        private readonly Mock<IWorkspaceRepository> _workspaceRepositoryMock = new();
        private readonly Mock<IIntegrationRepository> _integrationRepositoryMock = new();
        private readonly Mock<IRepositoryRepository> _repositoryRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private static readonly Guid WorkspaceId = Guid.Parse("9b591c5d-5b24-4920-b76b-89410003f522");
        private static readonly Guid IssuerId = Guid.Parse("ff813934-ae55-4fd2-aa71-946038a876b0");
        private static readonly Guid IntegrationDbId = Guid.Parse("f3983c62-6980-4606-a6f7-02fb782729a1");
        private static readonly Guid IntegrationAggregateId = Guid.Parse("69d55f2b-b957-40e9-9f8d-2ec5e8e9bd57");
        private const string IntegrationName = "Configured GitHub";
        private const string GitHubInstallationId = "123456";
        private const int RepositoryGitHubId = 9876;
        private const string RepositoryName = "flowjudge";
        private const string RepositoryFullName = "flowjudge/flowjudge";
        private static readonly DateTimeOffset CreationTimestamp = new(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);
    }
}
