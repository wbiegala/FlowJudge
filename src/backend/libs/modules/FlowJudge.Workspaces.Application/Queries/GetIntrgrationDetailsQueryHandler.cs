using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Application.Abstractions.Queries;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Domain.Workspace.Services;

namespace FlowJudge.Workspaces.Application.Queries
{
    internal sealed class GetIntrgrationDetailsQueryHandler : IQueryHandler<GetIntrgrationDetailsQuery, IntegrationData>
    {
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IIntegrationRepository _integrationRepository;
        private readonly IRepositoryRepository _repositoryRepository;

        public GetIntrgrationDetailsQueryHandler(
            IWorkspaceRepository workspaceRepository,
            IIntegrationRepository integrationRepository,
            IRepositoryRepository repositoryRepository)
        {
            _workspaceRepository = workspaceRepository;
            _integrationRepository = integrationRepository;
            _repositoryRepository = repositoryRepository;
        }

        public async Task<IResult<IntegrationData>> HandleAsync(
            GetIntrgrationDetailsQuery query,
            CancellationToken cancellationToken = default)
        {
            var workspaceId = WorkspaceId.Create(query.WorkspaceId);
            var canProcess = await VerifyPermissionsAsync(workspaceId, query.IssuerId, cancellationToken);
            if (!canProcess)
                return ApplicationResultFactory.Failure<IntegrationData>("Cannot view integrations.",
                    ErrorCodeGenerator.Forbidden("workspace"));

            var integrationId = IntegrationId.Create(query.IntegrationId);
            var integration = await _integrationRepository.GetIntegrationByAggregateIdAsync(integrationId, cancellationToken);

            if (integration is null)
                return ApplicationResultFactory.Failure<IntegrationData>("Integration not found.",
                    ErrorCodeGenerator.NotFound("integration"));

            var repositories = await _repositoryRepository.GetRepositoriesByIntegrationAsync(integrationId, cancellationToken);

            var result = new IntegrationData
            {
                IntegrationId = integration.AggregateId,
                WorkspaceId = integration.WorkspaceId,
                Name = integration.Name,
                Provider = integration.Provider,
                Status = integration.Status,
                CreatedAt = integration.CreatedAt,
                CreatedBy = integration.CreatedBy,
                Repositories = repositories.Select(r => new IntegrationData.IntegrationRepositoryListItem
                {
                    RepositoryId = r.AggregateId,
                    IntegrationId = r.IntegrationId,
                    WorkspaceId = r.WorkspaceId,
                    Name = r.Name,
                    FullName = r.FullName?.Value,
                    TrackingEnabled = r.TrackingEnabled,
                    Status = r.Status
                }).ToArray()
            };

            return ApplicationResultFactory.Success(result);
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
