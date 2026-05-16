using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Transactional;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Workspaces.Application.Abstractions.Commands;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Integration.Services;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.Services;

namespace FlowJudge.Workspaces.Application.Commands
{
    internal sealed class CreateIntegrationCommandHandler : TransactionalCommandHandler<CreateIntegrationCommand, Guid>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IIntegrationRepository _integrationRepository;
        private readonly IIntegrationFactory _integrationFactory;

        public CreateIntegrationCommandHandler(
            IWorkspaceRepository workspaceRepository,
            IIntegrationRepository integrationRepository,
            IIntegrationFactory integrationFactory,
            IUnitOfWork unitOfWork)
                : base(unitOfWork)
        {
            _workspaceRepository = workspaceRepository;
            _integrationRepository = integrationRepository;
            _integrationFactory = integrationFactory;
        }

        protected override async Task<IResult<Guid>> ExecuteInTransactionAsync(
            CreateIntegrationCommand command,
            CancellationToken cancellationToken = default)
        {
            var workspaceId = WorkspaceId.Create(command.WorkspaceId);
            var canProcess = await VerifyPermissionsAsync(workspaceId, command.IssuerId, cancellationToken);
            if (!canProcess)
                return ApplicationResultFactory.Failure<Guid>("Cannot create integration.",
                    ErrorCodeGenerator.Forbidden("workspace"));

            var integrationName = IntegrationName.Create(command.Name);

            IntegrationRoot? integration = command.Provider switch
            {
                IntegrationProvider.GitHub => _integrationFactory.CreateGithubIntegration(workspaceId, integrationName, command.IssuerId),
                _ => null
            };

            if (integration is null)
                return ApplicationResultFactory.Failure<Guid>("Unsupported integration provider.",
                    ErrorCodeGenerator.NotAcceptable("integration"));

            await _integrationRepository.AddIntegrationAsync(integration, cancellationToken);

            return ApplicationResultFactory.Success(integration.AggregateId.Value);
        }

        private async Task<bool> VerifyPermissionsAsync(WorkspaceId workspaceId, Guid userId, CancellationToken ct)
        {
            
            var role = await _workspaceRepository.GetUserRoleInWorkspaceAsync(workspaceId, userId, ct);

            if (!role.HasValue)
                return false;

            return WorkspaceRolePermissionsService.CanToAction(role.Value, Domain.WorkspacesBoundedContext.Actions.CreateIntegration);
        }
    }
}
