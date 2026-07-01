using System.Text.Json;
using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Cache;
using FlowJudge.GitHub.Client;
using FlowJudge.GitHub.Client.Clients;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Commands.GitHub;
using FlowJudge.Workspaces.Application.Services.GitHub;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using Moq;

namespace FlowJudge.Workspaces.UnitTests.Application.Services
{
    public class GitHubInstallationServiceTests
    {
        [Fact]
        public async Task StartGitHubInstallationAsync_WhenIssuerHasPermissions_ThenCreateInstallationState()
        {
            // Arrange
            GitHubInstallationModel? savedState = null;

            _workspaceRepositoryMock
                .Setup(wr => wr.GetUserRoleInWorkspaceAsync(
                    It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                    IssuerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(WorkspaceRole.Administrator);

            _githubServiceMock
                .Setup(gs => gs.GetInstallationUrlAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(GitHubInstallationUrl);

            _cacheMock
                .Setup(c => c.SetAsync(
                    It.IsAny<string>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<TimeSpan?>(),
                    It.IsAny<CancellationToken>()))
                .Callback<string, byte[], TimeSpan?, CancellationToken>((_, value, _, _) =>
                    savedState = JsonSerializer.Deserialize<GitHubInstallationModel>(value))
                .Returns(Task.CompletedTask);

            var service = CreateService();

            // Act
            var result = await service.StartGitHubInstallationAsync(WorkspaceId, IssuerId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotEqual(Guid.Empty, result.Data.InstallationStateId);
            Assert.Equal($"{GitHubInstallationUrl}?state={result.Data.InstallationStateId:N}", result.Data.RedirectUrl);
            Assert.NotNull(savedState);
            Assert.Equal(result.Data.InstallationStateId, savedState.StateId);
            Assert.Equal(WorkspaceId, savedState.WorkspaceId);
            Assert.Equal(IssuerId, savedState.IssuerId);
            _workspaceRepositoryMock.Verify(wr => wr.GetUserRoleInWorkspaceAsync(
                It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                IssuerId,
                It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(c => c.SetAsync(
                GetCacheKey(result.Data.InstallationStateId),
                It.IsAny<byte[]>(),
                TimeSpan.FromMinutes(60),
                It.IsAny<CancellationToken>()), Times.Once);
            _githubServiceMock.Verify(gs => gs.GetInstallationUrlAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task StartGitHubInstallationAsync_WhenIssuerHasInsufficientPermissions_ThenReturnFailure()
        {
            // Arrange
            _workspaceRepositoryMock
                .Setup(wr => wr.GetUserRoleInWorkspaceAsync(
                    It.Is<WorkspaceId>(id => id.Value == WorkspaceId),
                    IssuerId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(WorkspaceRole.Member);

            var service = CreateService();

            // Act
            var result = await service.StartGitHubInstallationAsync(WorkspaceId, IssuerId, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.True(result.Error.IsInsufficientPermissions());
            _cacheMock.Verify(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()), Times.Never);
            _githubServiceMock.Verify(gs => gs.GetInstallationUrlAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task ConfirmGitHubInstallationAsync_WhenInstallationStateExists_ThenCreateInactiveIntegrationAndRemoveState()
        {
            // Arrange
            CreateGitHubInstallationIntegrationCommand? sentCommand = null;
            var installationState = Fixture.CreateGitHubInstallationModel(
                InstallationStateId,
                WorkspaceId,
                IssuerId);

            _cacheMock
                .Setup(c => c.GetAsync(GetCacheKey(InstallationStateId), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ToCacheBytes(installationState));

            _mediatorMock
                .Setup(m => m.SendCommandAsync<CreateGitHubInstallationIntegrationCommand, Guid>(
                    It.IsAny<CreateGitHubInstallationIntegrationCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<CreateGitHubInstallationIntegrationCommand, CancellationToken>((command, _) => sentCommand = command)
                .ReturnsAsync(ApplicationResultFactory.Success(IntegrationAggregateId));

            var service = CreateService();

            // Act
            var result = await service.ConfirmGitHubInstallationAsync(
                InstallationStateId,
                GitHubInstallationId,
                setupAction: null,
                CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(WorkspaceId, result.Data.WorkspaceId);
            Assert.Equal(IntegrationAggregateId, result.Data.IntegrationId);
            Assert.NotNull(sentCommand);
            Assert.Equal(WorkspaceId, sentCommand.WorkspaceId);
            Assert.Equal(IssuerId, sentCommand.IssuerId);
            Assert.Equal(IntegrationName, sentCommand.Name);
            Assert.Equal(GitHubInstallationId, sentCommand.GitHubInstallationId);
            _cacheMock.Verify(c => c.GetAsync(GetCacheKey(InstallationStateId), It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(c => c.RemoveAsync(GetCacheKey(InstallationStateId), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task GetRepositoriesForInstallationAsync_WhenIntegrationExists_ThenReturnGitHubRepositories()
        {
            // Arrange
            var integration = CreateIntegrationWithInstallation();
            var repository = Fixture.CreateGitHubClientRepository(RepositoryGitHubId, RepositoryName, RepositoryFullName);
            var response = Fixture.CreateGitHubInstallationRepositoriesResponse(1, new[] { repository });

            _integrationRepositoryMock
                .Setup(ir => ir.GetIntegrationByAggregateIdAsync(
                    It.Is<IntegrationId>(id => id.Value == IntegrationAggregateId),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(integration);

            _repositoryClientMock
                .Setup(rc => rc.GetInstallationRepositoriesAsync(GitHubInstallationId, PageSize, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _githubServiceMock
                .Setup(gs => gs.GetClient<IRepositoryClient>())
                .Returns(_repositoryClientMock.Object);

            var service = CreateService();

            // Act
            var result = await service.GetRepositoriesForInstallationAsync(IntegrationAggregateId, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            var resultRepository = Assert.Single(result.Data);
            Assert.Equal(RepositoryGitHubId, resultRepository.Id);
            Assert.Equal(RepositoryName, resultRepository.Name);
            Assert.Equal(RepositoryFullName, resultRepository.FullName);
            _repositoryClientMock.Verify(rc => rc.GetInstallationRepositoriesAsync(
                GitHubInstallationId,
                PageSize,
                1,
                It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(c => c.SetAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CommitGitHubInstallationAsync_WhenIntegrationIsReady_ThenConfigureIntegration()
        {
            // Arrange
            ConfigureGitHubInstallationIntegrationCommand? sentCommand = null;
            var integration = CreateIntegrationWithInstallation();
            var repository = Fixture.CreateGitHubClientRepository(RepositoryGitHubId, RepositoryName, RepositoryFullName);
            var response = Fixture.CreateGitHubInstallationRepositoriesResponse(1, new[] { repository });
            var repositoryConfiguration = Fixture.CreateGitHubInstallationRepositoryConfiguration(
                RepositoryGitHubId,
                enableTracking: true);

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

            _repositoryClientMock
                .Setup(rc => rc.GetInstallationRepositoriesAsync(GitHubInstallationId, PageSize, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _githubServiceMock
                .Setup(gs => gs.GetClient<IRepositoryClient>())
                .Returns(_repositoryClientMock.Object);

            _mediatorMock
                .Setup(m => m.SendCommandAsync<ConfigureGitHubInstallationIntegrationCommand, Guid>(
                    It.IsAny<ConfigureGitHubInstallationIntegrationCommand>(),
                    It.IsAny<CancellationToken>()))
                .Callback<ConfigureGitHubInstallationIntegrationCommand, CancellationToken>((command, _) => sentCommand = command)
                .ReturnsAsync(ApplicationResultFactory.Success(IntegrationAggregateId));

            var service = CreateService();

            // Act
            var result = await service.CommitGitHubInstallationAsync(
                IntegrationAggregateId,
                IntegrationName,
                new[] { repositoryConfiguration },
                IssuerId,
                CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(IntegrationAggregateId, result.Data);
            Assert.NotNull(sentCommand);
            Assert.Equal(IntegrationAggregateId, sentCommand.IntegrationId);
            Assert.Equal(IssuerId, sentCommand.IssuerId);
            Assert.Equal(IntegrationName, sentCommand.Name);
            Assert.Equal(IntegrationStatus.Active, sentCommand.InitialStatus);
            var commandRepository = Assert.Single(sentCommand.Repositories);
            Assert.Equal(RepositoryGitHubId, commandRepository.GitHubId);
            Assert.Equal(RepositoryName, commandRepository.Name);
            Assert.Equal(RepositoryFullName, commandRepository.FullName);
            Assert.True(commandRepository.TrackingEnabled);
            _mediatorMock.Verify(m => m.SendCommandAsync<ConfigureGitHubInstallationIntegrationCommand, Guid>(
                It.IsAny<ConfigureGitHubInstallationIntegrationCommand>(),
                It.IsAny<CancellationToken>()), Times.Once);
            _cacheMock.Verify(c => c.RemoveAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task CommitGitHubInstallationAsync_WhenRepositoryDoesNotBelongToInstallation_ThenReturnFailure()
        {
            // Arrange
            var integration = CreateIntegrationWithInstallation();
            var repositoryConfiguration = Fixture.CreateGitHubInstallationRepositoryConfiguration(
                RepositoryGitHubId,
                enableTracking: true);
            var response = Fixture.CreateGitHubInstallationRepositoriesResponse(
                totalCount: 0,
                Enumerable.Empty<GitHub.Client.Contract.SharedModels.Repository>());

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

            _repositoryClientMock
                .Setup(rc => rc.GetInstallationRepositoriesAsync(GitHubInstallationId, PageSize, 1, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            _githubServiceMock
                .Setup(gs => gs.GetClient<IRepositoryClient>())
                .Returns(_repositoryClientMock.Object);

            var service = CreateService();

            // Act
            var result = await service.CommitGitHubInstallationAsync(
                IntegrationAggregateId,
                IntegrationName,
                new[] { repositoryConfiguration },
                IssuerId,
                CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.True(result.Error.IsNotFound());
            _mediatorMock.Verify(m => m.SendCommandAsync<ConfigureGitHubInstallationIntegrationCommand, Guid>(
                It.IsAny<ConfigureGitHubInstallationIntegrationCommand>(),
                It.IsAny<CancellationToken>()), Times.Never);
        }

        private GitHubInstallationService CreateService() =>
            new(
                _cacheMock.Object,
                _githubServiceMock.Object,
                _workspaceRepositoryMock.Object,
                _integrationRepositoryMock.Object,
                _mediatorMock.Object);

        private static GithubIntegration CreateIntegrationWithInstallation()
        {
            var integration = Fixture.CreateGithubIntegration(
                IntegrationDbId,
                IntegrationAggregateId,
                WorkspaceId,
                IntegrationName,
                IntegrationStatus.Inactive,
                CreationTimestamp,
                IssuerId);
            integration.UseInstallation(IntegrationAuthenticationValue.Create(GitHubInstallationId), CreationTimestamp, IssuerId);

            return integration;
        }

        private static byte[] ToCacheBytes<T>(T value) => JsonSerializer.SerializeToUtf8Bytes(value);

        private static string GetCacheKey(Guid stateId) => $":github-installation-state:{stateId:N}";

        private readonly Mock<IApplicationCache> _cacheMock = new();
        private readonly Mock<IGitHubService> _githubServiceMock = new();
        private readonly Mock<IWorkspaceRepository> _workspaceRepositoryMock = new();
        private readonly Mock<IIntegrationRepository> _integrationRepositoryMock = new();
        private readonly Mock<IMediator> _mediatorMock = new();
        private readonly Mock<IRepositoryClient> _repositoryClientMock = new();

        private static readonly Guid WorkspaceId = Guid.Parse("9b591c5d-5b24-4920-b76b-89410003f522");
        private static readonly Guid IssuerId = Guid.Parse("ff813934-ae55-4fd2-aa71-946038a876b0");
        private static readonly Guid InstallationStateId = Guid.Parse("2cbf3679-c82f-4f51-b50f-d50e8f3d601a");
        private static readonly Guid IntegrationDbId = Guid.Parse("f3983c62-6980-4606-a6f7-02fb782729a1");
        private static readonly Guid IntegrationAggregateId = Guid.Parse("69d55f2b-b957-40e9-9f8d-2ec5e8e9bd57");
        private const string GitHubInstallationId = "123456";
        private const string GitHubInstallationUrl = "https://github.com/apps/flowjudge/installations/new";
        private const string IntegrationName = "GitHub";
        private const int RepositoryGitHubId = 9876;
        private const string RepositoryName = "flowjudge";
        private const string RepositoryFullName = "flowjudge/flowjudge";
        private const int PageSize = 100;
        private static readonly DateTimeOffset CreationTimestamp = new(2026, 4, 1, 12, 0, 0, TimeSpan.Zero);
    }
}
