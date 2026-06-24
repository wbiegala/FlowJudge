using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Application.Abstractions.Ports
{
    public interface IIntegrationRepository
    {
        Task<IntegrationRoot?> GetIntegrationByAggregateIdAsync(IntegrationId integrationId, CancellationToken ct = default);
        Task AddIntegrationAsync(IntegrationRoot integration, CancellationToken ct = default);
        Task UpdateIntegrationAsync(IntegrationRoot integration, CancellationToken ct = default);
        Task<PagedList<IntegrationListItem>> GetIntegrationsAsync(WorkspaceId workspace, PageQuery pagination, CancellationToken ct = default);
        Task<IntegrationProvider?> GetIntegrationProviderAsync(IntegrationId integrationId, CancellationToken ct = default);

        Task<(WorkspaceId WorkspaceId, IntegrationId IntegrationId)?> GetIntegrationIdByGitHubInstallationIdAsync(
            string installationId,
            CancellationToken ct = default);
    }
}
