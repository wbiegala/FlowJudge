using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Transactional;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Workspaces.Application.Abstractions.Commands;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Application.Commands
{
    internal sealed class UpdateWorkspaceCommandHandler : TransactionalCommandHandler<UpdateWorkspaceCommand>
    {
        private readonly IWorkspaceRepository _workspaceRepository;

        public UpdateWorkspaceCommandHandler(IWorkspaceRepository workspaceRepository, IUnitOfWork unitOfWork)
            :base(unitOfWork)
        {
            _workspaceRepository = workspaceRepository;
        }

        protected override async Task<IResult> ExecuteInTransactionAsync(UpdateWorkspaceCommand command, CancellationToken cancellationToken = default)
        {
            var workspaceId = WorkspaceId.Create(command.WorkspaceId);
            var workspace = await _workspaceRepository.GetWorkspaceByAggregateIdAsync(workspaceId, cancellationToken);

            if (workspace is null)
                return ApplicationResultFactory.Failure("Workspace not found.",
                    ErrorCodeGenerator.NotFound(nameof(workspace)));

            if (workspace.Name != command.Name)
            {
                workspace.Rename(WorkspaceName.Create(command.Name), command.IssuerId);
            }

            await _workspaceRepository.UpdateWorkspaceAsync(workspace, cancellationToken);

            return ApplicationResultFactory.Success();
        }
    }
}
