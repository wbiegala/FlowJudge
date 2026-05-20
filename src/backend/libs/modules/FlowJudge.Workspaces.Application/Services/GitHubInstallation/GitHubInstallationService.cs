using FlowJudge.Common.Application;
using FlowJudge.Common.Cache;
using FlowJudge.GitHub.Client;
using FlowJudge.GitHub.Client.Clients;
using FlowJudge.GitHub.Client.Contract.SharedModels;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Abstractions.Services;
using FlowJudge.Workspaces.Application.Services.GitHubInstallation.Models;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.Services;
using ClientRepositoryContract = FlowJudge.GitHub.Client.Contract.SharedModels.Repository;

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
            CancellationToken ct)
        {
            var installationState = await _store.GetObjectAsync<GitHubInstallationModel>(GetCacheKey(installationStateId), ct);
            if (installationState is null)
                return ApplicationResultFactory.Failure<(Guid WorkspaceId, Guid InstallationStateId)>(
                    $"Installation state id={installationStateId} not found",
                    ErrorCodeGenerator.NotFound("installation-state"));

            installationState.InstallationId = installationId;

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

        public Task CommitGitHubInstallation(
            Guid installationStateId,
            string name,
            GitHubInstallationRepositoryConfiguration initialRepositoriesConfiguration,
            CancellationToken ct)
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

        private async Task<IEnumerable<ClientRepositoryContract>> GetAllInstallationRepositoriesAsync(
            string installationId,
            CancellationToken ct)
        {
            const int pageSize = 100;
         
            var client = _githubClientService.GetClient<IRepositoryClient>();
            var firstPage = await client.GetInstallationRepositoriesAsync(installationId, pageSize, 1, ct);

            if (firstPage.TotalCount <= pageSize)
                return firstPage.Repositiories;

            var result = new List<ClientRepositoryContract>();
            result.AddRange(firstPage.Repositiories);

            var totalCount = firstPage.TotalCount;
            var pagesCount = Math.Ceiling(decimal.Divide(totalCount, pageSize));

            for (var currentPage = 2; currentPage <= pagesCount; currentPage++)
            {
                if (ct.IsCancellationRequested)
                    break;
                var reposPage = await client.GetInstallationRepositoriesAsync(installationId, pageSize, currentPage, ct);
                result.AddRange(reposPage.Repositiories);
            }

            if (ct.IsCancellationRequested)
                return Enumerable.Empty<ClientRepositoryContract>();

            return result;
        }
    }
}
