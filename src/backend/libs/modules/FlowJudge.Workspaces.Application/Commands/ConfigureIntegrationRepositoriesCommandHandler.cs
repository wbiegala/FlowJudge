using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Transactional;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Workspaces.Application.Abstractions.Commands;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Repository.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.Services;
using Microsoft.Extensions.Logging;

namespace FlowJudge.Workspaces.Application.Commands
{
    internal sealed class ConfigureIntegrationRepositoriesCommandHandler
        : TransactionalCommandHandler<ConfigureIntegrationRepositoriesCommand>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IIntegrationRepository _integrationRepository;
        private readonly IRepositoryRepository _repositoryRepository;
        private readonly ILogger<ConfigureIntegrationRepositoriesCommandHandler> _logger;

        public ConfigureIntegrationRepositoriesCommandHandler(
            IWorkspaceRepository workspaceRepository,
            IIntegrationRepository integrationRepository,
            IRepositoryRepository repositoryRepository,
            ILogger<ConfigureIntegrationRepositoriesCommandHandler> logger,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _workspaceRepository = workspaceRepository;
            _integrationRepository = integrationRepository;
            _repositoryRepository = repositoryRepository;
            _logger = logger;
        }

        protected override async Task<IResult> ExecuteInTransactionAsync(
            ConfigureIntegrationRepositoriesCommand command,
            CancellationToken cancellationToken = default)
        {
            var workspaceId = WorkspaceId.Create(command.WorkspaceId);
            var canProcess = await VerifyPermissionsAsync(workspaceId, command.IssuerId, cancellationToken);
            if (!canProcess)
                return ApplicationResultFactory.Failure<Guid>("Cannot configure integration.",
                    ErrorCodeGenerator.Forbidden("integration"));

            var integrationId = IntegrationId.Create(command.IntegrationId);
            var integration = await _integrationRepository.GetIntegrationByAggregateIdAsync(integrationId, cancellationToken);

            if (integration is null)
                return ApplicationResultFactory.Failure<Guid>("Integration not found.",
                    ErrorCodeGenerator.NotFound("integration"));

            var storedRepositories = await _repositoryRepository.GetRepositoriesByIntegrationAsync(integrationId, cancellationToken);
            var repositories = storedRepositories is null ? new List<RepositoryRoot>() : storedRepositories.ToList();

            var processingResult = await ProcessRepositoriesAsync(repositories, command.Repositories, cancellationToken);

            if (processingResult.Any(r => r.Processed == false))
            {
                //TODO: for now we throw exception and rollback - maybe it should have partial success
                throw new InvalidOperationException($"");
            }

            return ApplicationResultFactory.Success();
        }

        private async Task<IEnumerable<(string RepositoryExternalId, bool Processed, string? Error)>> ProcessRepositoriesAsync(
            List<RepositoryRoot> integrationRepositories,
            IReadOnlyCollection<RepositoryConfiguration> repositoryConfigurations,
            CancellationToken ct)
        {
            var result = new List<(string RepositoryExternalId, bool Processed, string? Error)>();

            foreach (var repoCfg in repositoryConfigurations)
            {
                try
                {
                    var boundRepository = integrationRepositories.FirstOrDefault(repo => repo.ExternalId == repoCfg.ExternalId);

                    if (boundRepository is null)
                    {
                        var repository = await ProcessNewRepositoryAsync(repoCfg, ct);
                        integrationRepositories.Add(repository);
                    }
                    else
                    {
                        await ProcessExistingRepositoryAsync(boundRepository, repoCfg, ct);
                    }

                    result.Add(new(repoCfg.ExternalId, true, null));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Repository integrationId={IntegrationId} with ExternalId={ExternalId} failed on configuration.",
                        repoCfg.IntegrationId, repoCfg.ExternalId);
                    result.Add(new(repoCfg.ExternalId, false, ex.Message));
                }
            }

            result.AddRange(await ProcessNotConfiguredRepositoriesAsync(integrationRepositories, repositoryConfigurations, ct));

            return result;
        }

        private async Task<RepositoryRoot> ProcessNewRepositoryAsync(
            RepositoryConfiguration repositoryConfiguration,
            CancellationToken ct)
        {
            var repository = RepositoryRoot.Create(
                repositoryConfiguration.WorkspaceId,
                repositoryConfiguration.IntegrationId,
                repositoryConfiguration.ExternalId,
                repositoryConfiguration.Name,
                repositoryConfiguration.FullName,
                repositoryConfiguration.TrackingEnabled ?? false);

            await _repositoryRepository.AddRepositoryAsync(repository, ct);

            return repository;
        }

        private async Task ProcessExistingRepositoryAsync(
            RepositoryRoot repository,
            RepositoryConfiguration repositoryConfiguration,
            CancellationToken ct)
        {
            if (repository.Status == RepositoryStatus.Deleted)
                repository.Reactivate();

            repository.ChangeName(repositoryConfiguration.Name);
            repository.ChangeFullName(repositoryConfiguration.FullName);

            if (repositoryConfiguration.TrackingEnabled.HasValue)
            {
                if (repositoryConfiguration.TrackingEnabled == true)
                    repository.EnableTracking();
                else
                    repository.DisableTracking();
            }

            await _repositoryRepository.UpdateRepositoryAsync(repository, ct);
        }

        private async Task<IEnumerable<(string RepositoryExternalId, bool Processed, string? Error)>> ProcessNotConfiguredRepositoriesAsync(
            List<RepositoryRoot> integrationRepositories,
            IReadOnlyCollection<RepositoryConfiguration> repositoryConfigurations,
            CancellationToken ct)
        {
            var result = new List<(string RepositoryExternalId, bool Processed, string? Error)>();

            var repositoriesToDelete = integrationRepositories
                .Where(repo => !repositoryConfigurations.Any(cfg => cfg.ExternalId == repo.ExternalId));

            foreach (var repository in repositoriesToDelete)
            {
                try
                {
                    repository.Deactivate();
                    await _repositoryRepository.UpdateRepositoryAsync(repository, ct);
                    result.Add(new(repository.ExternalId, true, null));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Repository integrationId={IntegrationId} with ExternalId={ExternalId} failed on configuration.",
                        repository.IntegrationId, repository.ExternalId);
                    result.Add(new(repository.ExternalId, false, ex.Message));
                }
            }

            return result;
        }

        private async Task<bool> VerifyPermissionsAsync(WorkspaceId workspaceId, Guid userId, CancellationToken ct)
        {
            // if userId is empty, it means the command is issued by system, so we allow it to process.
            if (userId == Guid.Empty)
                return true;

            var role = await _workspaceRepository.GetUserRoleInWorkspaceAsync(workspaceId, userId, ct);

            if (!role.HasValue)
                return false;

            return WorkspaceRolePermissionsService.CanToAction(role.Value, Domain.WorkspacesBoundedContext.Actions.ConfigureIntegrationRepositories);
        }
    }
}
