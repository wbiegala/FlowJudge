using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Cache;
using FlowJudge.GitHub.Client;
using FlowJudge.GitHub.Client.Clients;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Abstractions.Services;
using FlowJudge.Workspaces.Application.Commands.Internals;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.Services;
using static FlowJudge.Workspaces.Application.Commands.Internals.ConfigureGitHubInstallationIntegrationCommand;
using ClientRepositoryContract = FlowJudge.GitHub.Client.Contract.SharedModels.Repository;

namespace FlowJudge.Workspaces.Application.Services.GitHubInstallation
{
    internal sealed class GitHubInstallationService : IGitHubInstallationService
    {
        private const int StoreTtlInMinutes = 60;
        private readonly IApplicationCache _store;
        private readonly IGitHubService _githubClientService;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IIntegrationRepository _integrationRepository;
        private readonly IMediator _mediator;

        public GitHubInstallationService(
            IApplicationCache applicationCache,
            IGitHubService githubClientService,
            IWorkspaceRepository workspaceRepository,
            IIntegrationRepository integrationRepository,
            IMediator mediator)
        {
            _store = applicationCache;
            _githubClientService = githubClientService;
            _workspaceRepository = workspaceRepository;
            _integrationRepository = integrationRepository;
            _mediator = mediator;
        }

        public async Task<IResult<(Guid InstallationStateId, string RedirectUrl)>> StartGitHubInstallationAsync(
            Guid workspaceId,
            Guid issuerId,
            CancellationToken ct)
        {
            var canProcess = await VerifyPermissionsAsync(WorkspaceId.Create(workspaceId), issuerId, ct);
            if (!canProcess)
                return ApplicationResultFactory.Failure<(Guid InstallationStateId, string RedirectUrl)>(
                    "Cannot connect integration.", ErrorCodeGenerator.Forbidden("workspace"));

            var installationState = new GitHubInstallationModel
            {
                StateId = Guid.NewGuid(),
                WorkspaceId = workspaceId,
                IssuerId = issuerId,
            };

            await _store.SetObjectAsync(GetCacheKey(installationState.StateId), installationState, TimeSpan.FromMinutes(StoreTtlInMinutes), ct);

            var baseUrl = await _githubClientService.GetInstallationUrlAsync(ct);
            var redirectUrl = $"{baseUrl}?state={installationState.StateId.ToString("N")}";

            return ApplicationResultFactory.Success<(Guid InstallationStateId, string RedirectUrl)>(new (installationState.StateId, redirectUrl));
        }

        public async Task<IResult<(Guid WorkspaceId, Guid IntegrationId)>> ConfirmGitHubInstallationAsync(
            Guid installationStateId,
            string installationId,
            string? setupAction,
            CancellationToken ct)
        {
            var installationState = await _store.GetObjectAsync<GitHubInstallationModel>(GetCacheKey(installationStateId), ct);
            if (installationState is null)
                return ApplicationResultFactory.Failure<(Guid WorkspaceId, Guid IntegrationId)>(
                    $"Installation state id={installationStateId} not found",
                    ErrorCodeGenerator.NotFound("installation-state"));

            var command = new CreateGitHubInstallationIntegrationCommand
            {
                WorkspaceId = installationState.WorkspaceId,
                IssuerId = installationState.IssuerId,
                Name = IntegrationProvider.GitHub.ToString(),
                GitHubInstallationId = installationId
            };

            var result = await _mediator.SendCommandAsync<CreateGitHubInstallationIntegrationCommand, Guid>(command, ct);
            if (!result.IsSuccess)
                return ApplicationResultFactory.Failure<(Guid WorkspaceId, Guid IntegrationId)>(result.Error!);

            await _store.RemoveAsync(GetCacheKey(installationState.StateId), ct);

            return ApplicationResultFactory.Success<(Guid WorkspaceId, Guid IntegrationId)>(
                new (installationState.WorkspaceId, result.Data));
        }

        public async Task<IResult<IEnumerable<GitHubInstallationRepository>>> GetRepositoriesForInstallationAsync(
            Guid integrationId,
            CancellationToken ct)
        {
            var integration = await _integrationRepository.GetIntegrationByAggregateIdAsync(IntegrationId.Create(integrationId), ct);
            if (integration is null)
                return ApplicationResultFactory.Failure<IEnumerable<GitHubInstallationRepository>>(
                    $"Integration id={integrationId} not found",
                    ErrorCodeGenerator.NotFound("integration"));

            var installationId = GetInstallationId(integration);
            if (string.IsNullOrWhiteSpace(installationId))
                return ApplicationResultFactory.Failure<IEnumerable<GitHubInstallationRepository>>(
                    $"Integration id={integrationId} has no GitHub installation id.",
                    ErrorCodeGenerator.NotAcceptable("integration"));

            var repositories = await GetAllInstallationRepositoriesAsync(installationId, ct);

            var result = repositories.Select(r => new GitHubInstallationRepository
            {
                Id = r.Id,
                Name = r.Name,
                FullName = r.FullName,
            }).ToArray();

            return ApplicationResultFactory.Success<IEnumerable<GitHubInstallationRepository>>(result);
        }

        public async Task<IResult<Guid>> CommitGitHubInstallationAsync(
            Guid integrationId,
            string name,
            IEnumerable<GitHubInstallationRepositoryConfiguration> initialRepositoriesConfiguration,
            Guid issuerId,
            CancellationToken ct)
        {
            var integration = await _integrationRepository.GetIntegrationByAggregateIdAsync(IntegrationId.Create(integrationId), ct);
            if (integration is null)
                return ApplicationResultFactory.Failure<Guid>(
                    $"Integration id={integrationId} not found",
                    ErrorCodeGenerator.NotFound("integration"));

            var canProcess = await VerifyPermissionsAsync(integration.WorkspaceId, issuerId, ct);
            if (!canProcess)
                return ApplicationResultFactory.Failure<Guid>(
                    "Cannot connect integration.", ErrorCodeGenerator.Forbidden("workspace"));

            var installationId = GetInstallationId(integration);
            if (string.IsNullOrWhiteSpace(installationId))
                return ApplicationResultFactory.Failure<Guid>(
                    $"Integration id={integrationId} has no GitHub installation id.",
                    ErrorCodeGenerator.NotAcceptable("integration"));

            var storedRepositories = (await GetAllInstallationRepositoriesAsync(installationId, ct)).ToDictionary(r => r.Id);
            var requestedRepositories = initialRepositoriesConfiguration.ToArray();
            var unknownRepository = requestedRepositories.FirstOrDefault(r => !storedRepositories.ContainsKey(r.GithubId));
            if (unknownRepository is not null)
                return ApplicationResultFactory.Failure<Guid>(
                    $"Repository id={unknownRepository.GithubId} not found in integration id={integrationId}.",
                    ErrorCodeGenerator.NotFound("integration-repository"));

            var command = new ConfigureGitHubInstallationIntegrationCommand
            {
                IntegrationId = integration.AggregateId.Value,
                IssuerId = issuerId,
                Name = name,
                InitialStatus = IntegrationStatus.Active,
                Repositories = requestedRepositories.Select(repo => new GitHubRepositoryInitialConfiguration
                {
                    GitHubId = repo.GithubId,
                    Name = storedRepositories[repo.GithubId].Name,
                    FullName = storedRepositories[repo.GithubId].FullName,
                    TrackingEnabled = repo.EnableTracking
                }).ToArray(),
            };

            return await _mediator.SendCommandAsync<ConfigureGitHubInstallationIntegrationCommand, Guid>(command, ct);
        }


        private static string GetCacheKey(Guid stateId) => $":github-installation-state:{stateId.ToString("N")}";

        private static string? GetInstallationId(IntegrationRoot integration)
        {
            return integration.AuthenticationData
                .Where(authentication =>
                    authentication.Type == IntegrationAuthenticationType.InstallationId &&
                    authentication.Status == IntegrationAuthenticationStatus.Active)
                .Select(authentication => authentication.Value.Value)
                .SingleOrDefault();
        }

        private async Task<bool> VerifyPermissionsAsync(WorkspaceId workspaceId, Guid userId, CancellationToken ct)
        {
            var role = await _workspaceRepository.GetUserRoleInWorkspaceAsync(workspaceId, userId, ct);

            if (!role.HasValue)
                return false;

            return WorkspaceRolePermissionsService.CanToAction(role.Value, Domain.WorkspacesBoundedContext.Actions.ConnectIntegration);
        }

        private async Task<IEnumerable<ClientRepositoryContract>> GetAllInstallationRepositoriesAsync(
            string installationId,
            CancellationToken ct)
        {
            const int pageSize = 100;
         
            var client = _githubClientService.GetClient<IRepositoryClient>();
            var firstPage = await client.GetInstallationRepositoriesAsync(installationId, pageSize, 1, ct);

            if (firstPage.TotalCount <= pageSize)
                return firstPage.Repositories;

            var result = new List<ClientRepositoryContract>();
            result.AddRange(firstPage.Repositories);

            var totalCount = firstPage.TotalCount;
            var pagesCount = (int)Math.Ceiling(decimal.Divide(totalCount, pageSize));

            for (var currentPage = 2; currentPage <= pagesCount; currentPage++)
            {
                var reposPage = await client.GetInstallationRepositoriesAsync(installationId, pageSize, currentPage, ct);
                result.AddRange(reposPage.Repositories);
            }

            return result;
        }
    }
}
