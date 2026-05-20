using FlowJudge.Common.Application;

namespace FlowJudge.Workspaces.Application.Abstractions.Services
{
    public interface IGitHubInstallationService
    {
        Task<IResult<(Guid InstallationStateId, string RedirectUrl)>> StartGitHubInstallationAsync(
            Guid workspaceId,
            Guid issuerId,
            CancellationToken ct);

        Task<IResult<(Guid WorkspaceId, Guid InstallationStateId)>> ConfirmGitHubInstallationAsync(
            Guid installationStateId,
            string installationId,
            string? setupAction,
            CancellationToken ct);

        Task<IResult<IEnumerable<GitHubInstallationRepository>>> GetRepositoriesForInstallationAsync(
            Guid installationStateId,
            CancellationToken ct);

        Task<IResult<Guid>> CommitGitHubInstallationAsync(
            Guid installationStateId,
            string name,
            IEnumerable<GitHubInstallationRepositoryConfiguration> initialRepositoriesConfiguration,
            Guid issuerId,
            CancellationToken ct);
    }

    public sealed record GitHubInstallationRepository
    {
        public int Id { get; init; }
        public required string Name { get; init; }
        public required string FullName { get; init; }
    }

    public sealed record GitHubInstallationRepositoryConfiguration
    {
        public int GithubId { get; init; }
        public bool EnableTracking { get; init; }
    }
}
