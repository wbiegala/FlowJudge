using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Transactional;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Repository.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.Services;

namespace FlowJudge.Workspaces.Application.Commands.GitHub
{
    internal sealed class ConfigureGitHubInstallationIntegrationCommandHandler
        : TransactionalCommandHandler<ConfigureGitHubInstallationIntegrationCommand, Guid>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IIntegrationRepository _integrationRepository;
        private readonly IRepositoryRepository _repositoryRepository;

        public ConfigureGitHubInstallationIntegrationCommandHandler(
            IWorkspaceRepository workspaceRepository,
            IIntegrationRepository integrationRepository,
            IRepositoryRepository repositoryRepository,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _workspaceRepository = workspaceRepository;
            _integrationRepository = integrationRepository;
            _repositoryRepository = repositoryRepository;
        }

        protected override async Task<IResult<Guid>> ExecuteInTransactionAsync(
            ConfigureGitHubInstallationIntegrationCommand command,
            CancellationToken cancellationToken = default)
        {
            var integration = await _integrationRepository.GetIntegrationByAggregateIdAsync(
                IntegrationId.Create(command.IntegrationId),
                cancellationToken);

            if (integration is null)
                return ApplicationResultFactory.Failure<Guid>(
                    $"Integration id={command.IntegrationId} not found.",
                    ErrorCodeGenerator.NotFound("integration"));

            var canProcess = await VerifyPermissionsAsync(integration.WorkspaceId, command.IssuerId, cancellationToken);
            if (!canProcess)
                return ApplicationResultFactory.Failure<Guid>("Cannot configure integration.",
                    ErrorCodeGenerator.Forbidden("workspace"));

            integration.Rename(IntegrationName.Create(command.Name));
            if (command.InitialStatus == IntegrationStatus.Active)
                integration.Activate();

            await _integrationRepository.UpdateIntegrationAsync(integration, cancellationToken);

            foreach (var githubRepository in command.Repositories)
            {
                var repositoryRoot = RepositoryRoot.Create(
                    workspaceId: integration.WorkspaceId.Value,
                    integrationId: integration.AggregateId.Value,
                    externalId: githubRepository.GitHubId.ToString(),
                    name: githubRepository.Name,
                    fullname: githubRepository.FullName,
                    trackingEnabled: githubRepository.TrackingEnabled);
                await _repositoryRepository.AddRepositoryAsync(repositoryRoot, cancellationToken);
            }

            return ApplicationResultFactory.Success(integration.AggregateId.Value);
        }

        private async Task<bool> VerifyPermissionsAsync(WorkspaceId workspaceId, Guid userId, CancellationToken ct)
        {
            var role = await _workspaceRepository.GetUserRoleInWorkspaceAsync(workspaceId, userId, ct);

            if (!role.HasValue)
                return false;

            return WorkspaceRolePermissionsService.CanToAction(role.Value, Domain.WorkspacesBoundedContext.Actions.ConnectIntegration);
        }
    }
}