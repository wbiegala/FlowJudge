using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Transactional;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Workspaces.Application.Abstractions.Commands;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.Services;
using static FlowJudge.Workspaces.Application.Abstractions.Commands.UpdateIntegrationCommand;

namespace FlowJudge.Workspaces.Application.Commands
{
    internal sealed class UpdateIntegrationCommandHandler : TransactionalCommandHandler<UpdateIntegrationCommand>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IIntegrationRepository _integrationRepository;
        private readonly IRepositoryRepository _repositoryRepository;

        public UpdateIntegrationCommandHandler(
            IWorkspaceRepository workspaceRepository,
            IIntegrationRepository integrationRepository,
            IRepositoryRepository repositoryRepository,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _workspaceRepository = workspaceRepository;
            _integrationRepository = integrationRepository;
            _repositoryRepository = repositoryRepository;
        }

        protected override async Task<IResult> ExecuteInTransactionAsync(
            UpdateIntegrationCommand command,
            CancellationToken cancellationToken = default)
        {
            var integrationId = IntegrationId.Create(command.IntegrationId);
            var integration = await _integrationRepository.GetIntegrationByAggregateIdAsync(integrationId, cancellationToken);

            if (integration is null)
                return ApplicationResultFactory.Failure<Guid>("Integration not found.",
                    ErrorCodeGenerator.NotFound("integration"));

            if (integration.WorkspaceId != command.WorkspaceId)
                return ApplicationResultFactory.Failure("Integration does not belong to workspace.",
                    ErrorCodeGenerator.NotFound("integration"));

            var canProcess = await VerifyPermissionsAsync(integration.WorkspaceId, command.IssuerId, cancellationToken);
            if (!canProcess)
                return ApplicationResultFactory.Failure<Guid>("Cannot update integration.",
                    ErrorCodeGenerator.Forbidden("integration"));

            if (integration.Name != command.Name)
            {
                integration.Rename(IntegrationName.Create(command.Name));
            }

            if (integration.Status != command.Status)
            {
                if (integration.Status == IntegrationStatus.Active && command.Status == IntegrationStatus.Inactive)
                    integration.Deactivate();

                if (integration.Status == IntegrationStatus.Inactive && command.Status == IntegrationStatus.Active)
                    integration.Activate();
            }

            await _integrationRepository.UpdateIntegrationAsync(integration, cancellationToken);

            await SetTrackingSettingsForRepositoriesAsync(
                integrationId,
                command.TrackingSettings,
                cancellationToken);

            return ApplicationResultFactory.Success();
        }

        private async Task SetTrackingSettingsForRepositoriesAsync(
            IntegrationId integrationId,
            IReadOnlyCollection<RepositoryTrackingSettings> trackingSettings,
            CancellationToken cancellationToken)
        {
            var integrationRepositories = await _repositoryRepository.GetRepositoriesByIntegrationAsync(integrationId, cancellationToken);

            foreach (var trackingSetting in trackingSettings)
            {
                var issuedRepository = integrationRepositories.FirstOrDefault(r => r.AggregateId == trackingSetting.RepositoryId);
                if (issuedRepository is null)
                {
                    throw new InvalidOperationException($"Repository with ID {trackingSetting.RepositoryId} not found in integration {integrationId.Value}.");
                }

                if (trackingSetting.TrackingEnabled)
                {
                    issuedRepository.EnableTracking();
                }
                else
                {
                    issuedRepository.DisableTracking();
                }

                await _repositoryRepository.UpdateRepositoryAsync(issuedRepository, cancellationToken);
            }
        }

        private async Task<bool> VerifyPermissionsAsync(WorkspaceId workspaceId, Guid userId, CancellationToken ct)
        {
            // if userId is empty, it means the command is issued by system, so we allow it to process.
            if (userId == Guid.Empty)
                return true;

            var role = await _workspaceRepository.GetUserRoleInWorkspaceAsync(workspaceId, userId, ct);

            if (!role.HasValue)
                return false;

            return WorkspaceRolePermissionsService.CanToAction(role.Value, Domain.WorkspacesBoundedContext.Actions.CreateIntegration);
        }
    }
}
