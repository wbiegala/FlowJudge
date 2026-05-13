using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Transactional;
using FlowJudge.Common.Domain;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Common.Utils.Time;
using FlowJudge.Workspaces.Application.Abstractions.Commands;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Application.Commands
{
    internal sealed class CreateWorkspaceCommandHandler : TransactionalCommandHandler<CreateWorkspaceCommand, Guid>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly ITimeService _timeService;

        public CreateWorkspaceCommandHandler(
            IWorkspaceRepository workspaceRepository,
            ITimeService timeService,
            IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            _workspaceRepository = workspaceRepository;
            _timeService = timeService;
        }

        protected override async Task<IResult<Guid>> ExecuteInTransactionAsync(CreateWorkspaceCommand command,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var workspace = WorkspaceRoot.Create(command.Name, command.CreatorId, _timeService.UtcNow);

                await _workspaceRepository.AddWorkspaceAsync(workspace, cancellationToken);

                return ApplicationResultFactory.Success(workspace.Id);
            }
            catch (DomainException ex)
            {
                return ApplicationResultFactory.Failure<Guid>(ex, ex.ErrorCode);
            }
            catch (Exception ex)
            {
                return ApplicationResultFactory.Failure<Guid>(ex.Message,
                    ErrorCodeGenerator.CreateFailed("workspace"));
            }
        }
    }
}
