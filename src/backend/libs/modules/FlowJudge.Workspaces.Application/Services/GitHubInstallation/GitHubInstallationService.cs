using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Cache;
using FlowJudge.GitHub.Client;
using FlowJudge.GitHub.Client.Clients;
using FlowJudge.GitHub.Client.Contract.SharedModels;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Abstractions.Services;
using FlowJudge.Workspaces.Application.Commands.Internals;
using FlowJudge.Workspaces.Application.Services.GitHubInstallation.Models;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.Services;
using static FlowJudge.Workspaces.Application.Commands.Internals.CreateGitHubInstallationIntegrationCommand;
using ClientRepositoryContract = FlowJudge.GitHub.Client.Contract.SharedModels.Repository;

namespace FlowJudge.Workspaces.Application.Services.GitHubInstallation
{
    internal sealed class GitHubInstallationService : IGitHubInstallationService
    {
        private const int StoreTtlInMinutes = 60;
        private readonly IApplicationCache _store;
        private readonly IGitHubService _githubClientService;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IMediator _mediator;

        public GitHubInstallationService(
            IApplicationCache applicationCache,
            IGitHubService githubClientService,
            IWorkspaceRepository workspaceRepository,
            IMediator mediator)
        {
            _store = applicationCache;
            _githubClientService = githubClientService;
            _workspaceRepository = workspaceRepository;
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

        public async Task<IResult<(Guid WorkspaceId, Guid InstallationStateId)>> ConfirmGitHubInstallationAsync(
            Guid installationStateId,
            string installationId,
            string? setupAction,
            CancellationToken ct)
        {
            var installationState = await _store.GetObjectAsync<GitHubInstallationModel>(GetCacheKey(installationStateId), ct);
            if (installationState is null)
                return ApplicationResultFactory.Failure<(Guid WorkspaceId, Guid InstallationStateId)>(
                    $"Installation state id={installationStateId} not found",
                    ErrorCodeGenerator.NotFound("installation-state"));

            installationState.InstallationId = installationId;
            installationState.SetupAction = setupAction;

            await _store.SetObjectAsync(GetCacheKey(installationState.StateId), installationState, TimeSpan.FromMinutes(StoreTtlInMinutes), ct);

            return ApplicationResultFactory.Success<(Guid WorkspaceId, Guid InstallationStateId)>(
                new (installationState.WorkspaceId, installationState.StateId));
        }

        public async Task<IResult<IEnumerable<GitHubInstallationRepository>>> GetRepositoriesForInstallationAsync(
            Guid installationStateId,
            CancellationToken ct)
        {
            var installationState = await _store.GetObjectAsync<GitHubInstallationModel>(GetCacheKey(installationStateId), ct);
            if (installationState is null)
                return ApplicationResultFactory.Failure<IEnumerable<GitHubInstallationRepository>>(
                    $"Installation state id={installationStateId} not found",
                    ErrorCodeGenerator.NotFound("installation-state"));

            if (string.IsNullOrWhiteSpace(installationState.InstallationId))
                return ApplicationResultFactory.Failure<IEnumerable<GitHubInstallationRepository>>(
                    $"Installation state id={installationStateId} has no GitHub installation id.",
                    ErrorCodeGenerator.NotAcceptable("installation-state"));

            var repositories = await GetAllInstallationRepositoriesAsync(installationState.InstallationId, ct);

            installationState.Repositories = repositories
                .Select(r => new GitHubInstallationModel.GitHubRepository
                {
                    Id = r.Id,
                    Name = r.Name,
                    FullName = r.FullName,
                }).ToArray();

            await _store.SetObjectAsync(GetCacheKey(installationState.StateId), installationState, TimeSpan.FromMinutes(StoreTtlInMinutes), ct);
            var result = repositories.Select(r => new GitHubInstallationRepository
            {
                Id = r.Id,
                Name = r.Name,
                FullName = r.FullName,
            }).ToArray();

            return ApplicationResultFactory.Success<IEnumerable<GitHubInstallationRepository>>(result);
        }

        public async Task<IResult<Guid>> CommitGitHubInstallationAsync(
            Guid installationStateId,
            string name,
            IEnumerable<GitHubInstallationRepositoryConfiguration> initialRepositoriesConfiguration,
            Guid issuerId,
            CancellationToken ct)
        {
            var installationState = await _store.GetObjectAsync<GitHubInstallationModel>(GetCacheKey(installationStateId), ct);
            if (installationState is null)
                return ApplicationResultFactory.Failure<Guid>(
                    $"Installation state id={installationStateId} not found",
                    ErrorCodeGenerator.NotFound("installation-state"));

            var canProcess = await VerifyPermissionsAsync(WorkspaceId.Create(installationState.WorkspaceId), issuerId, ct);
            if (!canProcess)
                return ApplicationResultFactory.Failure<Guid>(
                    "Cannot connect integration.", ErrorCodeGenerator.Forbidden("workspace"));

            var getStoredRepoData = (int repoId) => installationState.Repositories!.FirstOrDefault(r => r.Id == repoId);
           

            var command = new CreateGitHubInstallationIntegrationCommand
            {
                WorkspaceId = installationState.WorkspaceId,
                IssuerId = issuerId,
                Name = name,
                GitHubInstallationId = installationState.InstallationId!,
                InitialStatus = installationState.SetupAction switch
                {
                    "install" => IntegrationStatus.Active,
                    _ => IntegrationStatus.Inactive
                },
                Repositories = initialRepositoriesConfiguration.Select(repo => new GitHubRepositoryInitialConfiguration
                {
                    GitHubId = repo.GithubId,
                    Name = getStoredRepoData(repo.GithubId).Name,
                    FullName = getStoredRepoData(repo.GithubId).FullName,
                    TrackingEnabled = repo.EnableTracking
                }).ToArray(),
            };

            var result = await _mediator.SendCommandAsync<CreateGitHubInstallationIntegrationCommand, Guid>(command, ct);

            return result;
        }


        private static string GetCacheKey(Guid stateId) => $":github-installation-state:{stateId.ToString("N")}";

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
