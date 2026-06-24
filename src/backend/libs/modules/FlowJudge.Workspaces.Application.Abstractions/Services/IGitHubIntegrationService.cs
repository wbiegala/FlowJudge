using FlowJudge.Common.Application;

namespace FlowJudge.Workspaces.Application.Abstractions.Services
{
    public interface IGitHubIntegrationService
    {
        Task<IResult> UpdateIntegrationAsync(string installationId, CancellationToken ct = default);
        Task<IResult> ActivateIntegrationAsync(string installationId, CancellationToken ct = default);
        Task<IResult> DeactivateIntegrationAsync(string installationId, CancellationToken ct = default);
        Task<IResult> DeleteIntegrationAsync(string installationId, CancellationToken ct = default);
    }
}
