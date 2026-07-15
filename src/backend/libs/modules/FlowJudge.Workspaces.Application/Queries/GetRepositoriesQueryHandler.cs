using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Abstractions.Queries;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.Services;

namespace FlowJudge.Workspaces.Application.Queries
{
    internal sealed class GetRepositoriesQueryHandler : IQueryHandler<GetRepositoriesQuery, PagedList<RepositoryListItem>>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IRepositoryRepository _repositoryRepository;

        public GetRepositoriesQueryHandler(
            IWorkspaceRepository workspaceRepository,
            IRepositoryRepository repositoryRepository)
        {
            _workspaceRepository = workspaceRepository;
            _repositoryRepository = repositoryRepository;
        }

        public async Task<IResult<PagedList<RepositoryListItem>>> HandleAsync(
            GetRepositoriesQuery query,
            CancellationToken cancellationToken = default)
        {
            var workspaceId = WorkspaceId.Create(query.WorkspaceId);
            var canProcess = await VerifyPermissionsAsync(workspaceId, query.IssuerId, cancellationToken);
            if (!canProcess)
                return ApplicationResultFactory.Failure<PagedList<RepositoryListItem>>("Cannot view repositories.",
                    ErrorCodeGenerator.Forbidden("workspace"));

            var repositories = await _repositoryRepository.GetRepositoriesAsync(workspaceId, query.Pagination, cancellationToken);

            return ApplicationResultFactory.Success(repositories);
        }

        private async Task<bool> VerifyPermissionsAsync(WorkspaceId workspaceId, Guid userId, CancellationToken ct)
        {
            var role = await _workspaceRepository.GetUserRoleInWorkspaceAsync(workspaceId, userId, ct);

            if (!role.HasValue)
                return false;

            return WorkspaceRolePermissionsService.CanToAction(role.Value, Domain.WorkspacesBoundedContext.Actions.ViewIntegration);
        }
    }
}
