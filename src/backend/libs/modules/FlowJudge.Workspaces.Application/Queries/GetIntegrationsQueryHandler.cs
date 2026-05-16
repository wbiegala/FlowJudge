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
    internal sealed class GetIntegrationsQueryHandler : IQueryHandler<GetIntegrationsQuery, PagedList<IntegrationListItem>>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IIntegrationRepository _integrationRepository;

        public GetIntegrationsQueryHandler(
            IWorkspaceRepository workspaceRepository,
            IIntegrationRepository integrationRepository)
        {
            _workspaceRepository = workspaceRepository;
            _integrationRepository = integrationRepository;
        }

        public async Task<IResult<PagedList<IntegrationListItem>>> HandleAsync(
            GetIntegrationsQuery query,
            CancellationToken cancellationToken = default)
        {
            var workspaceId = WorkspaceId.Create(query.WorkspaceId);
            var canProcess = await VerifyPermissionsAsync(workspaceId, query.IssuerId, cancellationToken);
            if (!canProcess)
                return ApplicationResultFactory.Failure<PagedList<IntegrationListItem>>("Cannot view integrations.",
                    ErrorCodeGenerator.Forbidden("workspace"));

            var integrations = await _integrationRepository.GetIntegrationsAsync(workspaceId, query.Pagination, cancellationToken);

            return ApplicationResultFactory.Success(integrations);
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
