using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.GitHub.Client;
using FlowJudge.GitHub.Client.Clients;
using FlowJudge.GitHub.Client.Extensions;
using FlowJudge.Workspaces.Application.Abstractions.Commands;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Abstractions.Services;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Application.Services.GitHub
{
    internal sealed class GitHubIntegrationService : IGitHubIntegrationService
    {
        private readonly IIntegrationRepository _repository;
        private readonly IGitHubService _gitHubService;
        private readonly IMediator _mediator;

        public GitHubIntegrationService(
            IIntegrationRepository repository,
            IGitHubService gitHubService,
            IMediator mediator)
        {
            _repository = repository;
            _gitHubService = gitHubService;
            _mediator = mediator;
        }

        public async Task<IResult> UpdateIntegrationAsync(string installationId, CancellationToken ct = default)
        {
            var integrationId = await GetIntegrationIdByInstallationIdAsync(installationId, ct);
            if (!integrationId.HasValue)
                return ApplicationResultFactory.Failure(
                    $"Integration not found for GitHub installation Id '{installationId}'", ErrorCodeGenerator.NotFound("integration"));

            var client = _gitHubService.GetClient<IRepositoryClient>();
            var installationRepositories = await client.GetAllInstallationRepositoriesAsync(installationId, ct);

            var command = new ConfigureIntegrationRepositoriesCommand(
                WorkspaceId: integrationId.Value.WorkspaceId,
                IntegrationId: integrationId.Value.IntegrationId,
                IssuerId: Guid.Empty, // No specific issuer, as this is triggered by GitHub webhook
                Repositories: installationRepositories.Select(repo => new RepositoryConfiguration
                {
                    WorkspaceId = integrationId.Value.WorkspaceId,
                    IntegrationId = integrationId.Value.IntegrationId,
                    RepositoryId = null,
                    ExternalId = repo.Id.ToString(),
                    Name = repo.Name,
                    FullName = repo.FullName,
                    TrackingEnabled = null,
                }).ToList());

            return await _mediator.SendCommandAsync(command, ct);
        }

        public async Task<IResult> ActivateIntegrationAsync(string installationId, CancellationToken ct = default)
        {
            var integrationId = await GetIntegrationIdByInstallationIdAsync(installationId, ct);
            if (!integrationId.HasValue)
                return ApplicationResultFactory.Failure(
                    $"Integration not found for GitHub installation Id '{installationId}'", ErrorCodeGenerator.NotFound("integration"));

            var command = new ActivateIntegrationCommand(
                integrationId.Value.WorkspaceId,
                integrationId.Value.IntegrationId,
                Guid.Empty); // No specific issuer, as this is triggered by GitHub webhook

            return await _mediator.SendCommandAsync(command, ct);
        }

        public async Task<IResult> DeactivateIntegrationAsync(string installationId, CancellationToken ct = default)
        {
            var integrationId = await GetIntegrationIdByInstallationIdAsync(installationId, ct);
            if (!integrationId.HasValue)
                return ApplicationResultFactory.Failure(
                    $"Integration not found for GitHub installation Id '{installationId}'", ErrorCodeGenerator.NotFound("integration"));

            var command = new DeactivateIntegrationCommand(
                WorkspaceId: integrationId.Value.WorkspaceId,
                IntegrationId: integrationId.Value.IntegrationId,
                IssuerId: Guid.Empty); // No specific issuer, as this is triggered by GitHub webhook

            return await _mediator.SendCommandAsync(command, ct);
        }

        public async Task<IResult> DeleteIntegrationAsync(string installationId, CancellationToken ct = default)
        {
            var integrationId = await GetIntegrationIdByInstallationIdAsync(installationId, ct);
            if (!integrationId.HasValue)
                return ApplicationResultFactory.Failure(
                    $"Integration not found for GitHub installation Id '{installationId}'", ErrorCodeGenerator.NotFound("integration"));

            var command = new DeleteIntegrationCommand(
                WorkspaceId: integrationId.Value.WorkspaceId,
                IntegrationId: integrationId.Value.IntegrationId,
                IssuerId: Guid.Empty // No specific issuer, as this is triggered by GitHub webhook
            );

            return await _mediator.SendCommandAsync(command, ct);
        }

        private async Task<(WorkspaceId WorkspaceId, IntegrationId IntegrationId)?> GetIntegrationIdByInstallationIdAsync(
            string installationId,
            CancellationToken ct = default)
        {
            var integrationId = await _repository.GetIntegrationIdByGitHubInstallationIdAsync(installationId, ct);
            if (integrationId == null)
                return null;      

            return integrationId.Value;
        }
    }
}
