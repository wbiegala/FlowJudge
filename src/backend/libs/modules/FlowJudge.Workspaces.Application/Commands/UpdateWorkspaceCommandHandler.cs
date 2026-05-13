using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Domain;
using FlowJudge.Workspaces.Application.Abstractions.Commands;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Application.Commands
{
    internal sealed class UpdateWorkspaceCommandHandler : ICommandHandler<UpdateWorkspaceCommand>
    {
        private readonly IWorkspaceRepository _workspaceRepository;

        public UpdateWorkspaceCommandHandler(IWorkspaceRepository workspaceRepository)
        {
            _workspaceRepository = workspaceRepository;
        }

        public async Task<IResult> ExecuteAsync(
            UpdateWorkspaceCommand command,
            CancellationToken cancellationToken = default)
        {
            var workspaceId = WorkspaceId.Create(command.WorkspaceId);
            var workspace = await _workspaceRepository.GetWorkspaceByAggregateIdAsync(workspaceId, cancellationToken);

            if (workspace is null)
                return ApplicationResultFactory.Failure("Workspace not found.",
                    ErrorCodeGenerator.NotFound(nameof(workspace)));

            try
            {
                if (workspace.Name != command.Name)
                {
                    workspace.Rename(WorkspaceName.Create(command.Name), command.IssuerId);
                }

                await _workspaceRepository.UpdateWorkspaceAsync(workspace, cancellationToken);

                return ApplicationResultFactory.Success();
            }
            catch (DomainException ex)
            {
                return ApplicationResultFactory.Failure(ex.Message, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return ApplicationResultFactory.Failure(ex,
                    ErrorCodeGenerator.UpdateFailed(nameof(workspace)));
            }
        }
    }
}
