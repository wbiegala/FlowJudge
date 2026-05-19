using FlowJudge.Common.Application;

namespace FlowJudge.Workspaces.Application.Abstractions.Services
{
    public interface IGitHubInstallationService
    {
        Task<IResult<(Guid InstallationStateId, string RedirectUrl)>> StartGitHubInstallationAsync(
            Guid workspaceId,
            Guid issuerId,
            CancellationToken ct);

        Task<IResult<Guid>> ConfirmGitHubInstallationAsync(
            Guid installationStateId,
            string installationId,
            CancellationToken ct);

        Task<IResult<GitHubInstallationRepository>> GetRepositoriesForInstallationAsync(
            Guid installationStateId,
            CancellationToken ct);

        Task CommitGitHubInstallation(
            Guid installationStateId,
            string name,
            GitHubInstallationRepositoryConfiguration initialRepositoriesConfiguration,
            CancellationToken ct);
    }

    public sealed record GitHubInstallationRepository
    {

    }

    public sealed record GitHubInstallationRepositoryConfiguration
    {

    }
}
