using FlowJudge.Common.Application;
using FlowJudge.Common.Cache;
using FlowJudge.GitHub.Client;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Abstractions.Services;
using FlowJudge.Workspaces.Application.Services.GitHubInstallation.Models;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.Services;

namespace FlowJudge.Workspaces.Application.Services.GitHubInstallation
{
    internal sealed class GitHubInstallationService : IGitHubInstallationService
    {
        private const int StoreTtlInMinutes = 60;
        private readonly IApplicationCache _store;
        private readonly IGitHubService _githubClientService;
        private readonly IWorkspaceRepository _workspaceRepository;

        public GitHubInstallationService(
            IApplicationCache applicationCache,
            IGitHubService githubClientService,
            IWorkspaceRepository workspaceRepository)
        {
            _store = applicationCache;
            _githubClientService = githubClientService;
            _workspaceRepository = workspaceRepository;
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

            var installationState = new GitHubInstallationState
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

        public async Task<IResult<Guid>> ConfirmGitHubInstallationAsync(Guid installationStateId, string installationId, CancellationToken ct)
        {
            var installationState = await _store.GetObjectAsync<GitHubInstallationState>(GetCacheKey(installationStateId), ct);
            if (installationState is null)
                return ApplicationResultFactory.Failure<Guid>(
                    $"Installation state id={installationStateId} not found",
                    ErrorCodeGenerator.NotFound("installation-state"));

            installationState.InstallationId = installationId;

            await _store.SetObjectAsync(GetCacheKey(installationState.StateId), installationState, TimeSpan.FromMinutes(StoreTtlInMinutes), ct);

            return ApplicationResultFactory.Success(installationState.StateId);
        }

        public Task<IResult<GitHubInstallationRepository>> GetRepositoriesForInstallationAsync(Guid installationStateId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task CommitGitHubInstallation(Guid installationStateId, string name, GitHubInstallationRepositoryConfiguration initialRepositoriesConfiguration, CancellationToken ct)
        {
            throw new NotImplementedException();
        }


        private static string GetCacheKey(Guid stateId) => $":github-installation-state:{stateId.ToString("N")}";

        private async Task<bool> VerifyPermissionsAsync(WorkspaceId workspaceId, Guid userId, CancellationToken ct)
        {
            var role = await _workspaceRepository.GetUserRoleInWorkspaceAsync(workspaceId, userId, ct);

            if (!role.HasValue)
                return false;

            return WorkspaceRolePermissionsService.CanToAction(role.Value, Domain.WorkspacesBoundedContext.Actions.ConnectIntegration);
        }


    }
}
