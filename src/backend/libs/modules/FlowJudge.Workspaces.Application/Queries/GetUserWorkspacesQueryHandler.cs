using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Abstractions.Queries;

namespace FlowJudge.Workspaces.Application.Queries
{
    internal sealed class GetUserWorkspacesQueryHandler : IQueryHandler<GetUserWorkspacesQuery, PagedList<WorkspaceListItem>>
    {
        private readonly IWorkspaceRepository _workspaceRepository;

        public GetUserWorkspacesQueryHandler(IWorkspaceRepository workspaceRepository)
        {
            _workspaceRepository = workspaceRepository;
        }

        public async Task<IResult<PagedList<WorkspaceListItem>>> HandleAsync(
            GetUserWorkspacesQuery query,
            CancellationToken cancellationToken = default)
        {
            var workspaces = await _workspaceRepository.GetUserWorkspacesAsync(
                query.UserId,
                query.Pagination,
                cancellationToken);

            return ApplicationResultFactory.Success(workspaces);
        }
    }
}
