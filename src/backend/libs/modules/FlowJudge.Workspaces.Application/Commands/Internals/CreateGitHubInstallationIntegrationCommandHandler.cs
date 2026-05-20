using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Transactional;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Integration.Services;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.Services;

namespace FlowJudge.Workspaces.Application.Commands.Internals
{
    internal sealed class CreateGitHubInstallationIntegrationCommandHandler
        : TransactionalCommandHandler<CreateGitHubInstallationIntegrationCommand, Guid>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IIntegrationRepository _integrationRepository;
        private readonly IIntegrationFactory _integrationFactory;

        public CreateGitHubInstallationIntegrationCommandHandler(
            IWorkspaceRepository workspaceRepository,
            IIntegrationRepository integrationRepository,
            IIntegrationFactory integrationFactory,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _workspaceRepository = workspaceRepository;
            _integrationRepository = integrationRepository;
            _integrationFactory = integrationFactory;
        }

        protected override async Task<IResult<Guid>> ExecuteInTransactionAsync(
            CreateGitHubInstallationIntegrationCommand command,
            CancellationToken cancellationToken = default)
        {
            var workspaceId = WorkspaceId.Create(command.WorkspaceId);
            var canProcess = await VerifyPermissionsAsync(workspaceId, command.IssuerId, cancellationToken);
            if (!canProcess)
                return ApplicationResultFactory.Failure<Guid>("Cannot create integration.",
                    ErrorCodeGenerator.Forbidden("workspace"));

            var integrationName = IntegrationName.Create(command.Name);
            var integrationRoot = _integrationFactory.CreateGithubIntegration(workspaceId, integrationName, command.IssuerId);
            integrationRoot.UseInstallation(IntegrationAuthenticationValue.Create(command.GitHubInstallationId), integrationRoot.CreatedAt, command.IssuerId);

            if (command.InitialStatus == IntegrationStatus.Active)
                integrationRoot.Activate();

            await _integrationRepository.AddIntegrationAsync(integrationRoot, cancellationToken);


            //TODO - repozytoria!


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
