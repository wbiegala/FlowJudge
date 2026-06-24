using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Transactional;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Workspaces.Application.Abstractions.Commands;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.Services;

namespace FlowJudge.Workspaces.Application.Commands
{
    internal sealed class ActivateIntegrationCommandHandler : TransactionalCommandHandler<ActivateIntegrationCommand>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IIntegrationRepository _integrationRepository;

        public ActivateIntegrationCommandHandler(
            IWorkspaceRepository workspaceRepository,
            IIntegrationRepository integrationRepository,
            IUnitOfWork unitOfWork)
                : base(unitOfWork)
        {
            _workspaceRepository = workspaceRepository;
            _integrationRepository = integrationRepository;
        }

        protected override async Task<IResult> ExecuteInTransactionAsync(
            ActivateIntegrationCommand command,
            CancellationToken cancellationToken = default)
        {
            var workspaceId = WorkspaceId.Create(command.WorkspaceId);
            var canProcess = await VerifyPermissionsAsync(workspaceId, command.IssuerId, cancellationToken);
            if (!canProcess)
                return ApplicationResultFactory.Failure<Guid>("Cannot activate integration.",
                    ErrorCodeGenerator.Forbidden("integration"));
            
            var integrationId = IntegrationId.Create(command.IntegrationId);
            var integration = await _integrationRepository.GetIntegrationByAggregateIdAsync(integrationId, cancellationToken);

            if (integration is null)
                return ApplicationResultFactory.Failure<Guid>("Integration not found.",
                    ErrorCodeGenerator.NotFound("integration"));

            try
            {
                integration.Activate();

                await _integrationRepository.UpdateIntegrationAsync(integration, cancellationToken);

                return ApplicationResultFactory.Success();
            }
            catch (Exception ex)
            {
                return ApplicationResultFactory.Failure(ex, ErrorCodeGenerator.UpdateFailed("integration"));
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

            return WorkspaceRolePermissionsService.CanToAction(role.Value, Domain.WorkspacesBoundedContext.Actions.ActivateIntegration);
        }
    }
}
