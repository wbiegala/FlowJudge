using FlowJudge.Common.Utils.Pagination;
using FlowJudge.GitHub.Client.Contract;
using FlowJudge.GitHub.Client.Contract.SharedModels;
using FlowJudge.Workspaces.Application.Abstractions.Commands;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Queries;
using FlowJudge.Workspaces.Application.Abstractions.Services;
using FlowJudge.Workspaces.Application.Commands.GitHub;
using FlowJudge.Workspaces.Application.Services.GitHub;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Repository.Model;
using static FlowJudge.Workspaces.Application.Commands.GitHub.ConfigureGitHubInstallationIntegrationCommand;

namespace FlowJudge.Workspaces.UnitTests.Application
{
    internal static class Fixture
    {
        public static CreateIntegrationCommand CreateIntegrationCommand(
            string name,
            IntegrationProvider provider,
            Guid workspaceId,
            Guid issuerId) =>
            new(name, provider, workspaceId, issuerId);

        public static GetIntegrationsQuery GetIntegrationsQuery(
            Guid workspaceId,
            Guid issuerId,
            PageQuery pagination) =>
            new(workspaceId, issuerId, pagination);

        public static CreateGitHubInstallationIntegrationCommand CreateGitHubInstallationIntegrationCommand(
            Guid workspaceId,
            Guid issuerId,
            string name,
            string gitHubInstallationId) =>
            new()
            {
                WorkspaceId = workspaceId,
                IssuerId = issuerId,
                Name = name,
                GitHubInstallationId = gitHubInstallationId
            };

        public static ConfigureGitHubInstallationIntegrationCommand ConfigureGitHubInstallationIntegrationCommand(
            Guid integrationId,
            Guid issuerId,
            string name,
            IntegrationStatus initialStatus,
            IEnumerable<GitHubRepositoryInitialConfiguration> repositories) =>
            new()
            {
                IntegrationId = integrationId,
                IssuerId = issuerId,
                Name = name,
                InitialStatus = initialStatus,
                Repositories = repositories
            };

        public static ConfigureIntegrationRepositoriesCommand ConfigureIntegrationRepositoriesCommand(
            Guid workspaceId,
            Guid integrationId,
            Guid issuerId,
            IReadOnlyCollection<RepositoryConfiguration> repositories) =>
            new(workspaceId, integrationId, issuerId, repositories);

        public static GithubIntegration CreateGithubIntegration(
            Guid id,
            Guid aggregateId,
            Guid workspaceId,
            string name,
            IntegrationStatus status,
            DateTimeOffset createdAt,
            Guid createdBy) =>
            GithubIntegration.Load(
                id,
                aggregateId,
                workspaceId,
                name,
                status,
                Enumerable.Empty<IntegrationAuthentication>(),
                createdAt,
                createdBy);

        public static RepositoryRoot CreateRepository(
            Guid id,
            Guid aggregateId,
            Guid workspaceId,
            Guid integrationId,
            string externalId,
            string name,
            string? fullName,
            bool trackingEnabled,
            RepositoryStatus status = RepositoryStatus.Active) =>
            RepositoryRoot.Load(
                id,
                aggregateId,
                workspaceId,
                integrationId,
                externalId,
                name,
                fullName,
                trackingEnabled,
                status.ToString());

        public static RepositoryConfiguration CreateRepositoryConfiguration(
            Guid workspaceId,
            Guid integrationId,
            string externalId,
            string name,
            string? fullName,
            bool? trackingEnabled) =>
            new()
            {
                WorkspaceId = workspaceId,
                IntegrationId = integrationId,
                ExternalId = externalId,
                Name = name,
                FullName = fullName,
                TrackingEnabled = trackingEnabled
            };

        public static IntegrationListItem CreateIntegrationListItem(
            Guid id,
            string name,
            IntegrationProvider provider,
            IntegrationStatus status,
            DateTimeOffset createdAt,
            Guid createdBy) =>
            new()
            {
                Id = id,
                Name = name,
                Provider = provider,
                Status = status,
                CreatedAt = createdAt,
                CreatedBy = createdBy
            };

        public static PagedList<IntegrationListItem> CreateIntegrationPagedList(
            IEnumerable<IntegrationListItem> items,
            int pageSize,
            int pageNumber,
            int totalCount) =>
            new(items, pageSize, pageNumber, totalCount);

        public static PageQuery CreatePageQuery(int pageSize, int pageNumber) =>
            new()
            {
                PageSize = pageSize,
                PageNumber = pageNumber
            };

        public static GitHubRepositoryInitialConfiguration CreateGitHubRepositoryInitialConfiguration(
            int gitHubId,
            string name,
            string fullName,
            bool trackingEnabled) =>
            new()
            {
                GitHubId = gitHubId,
                Name = name,
                FullName = fullName,
                TrackingEnabled = trackingEnabled
            };

        public static GitHubInstallationModel CreateGitHubInstallationModel(
            Guid stateId,
            Guid workspaceId,
            Guid issuerId) =>
            new()
            {
                StateId = stateId,
                WorkspaceId = workspaceId,
                IssuerId = issuerId
            };

        public static GitHubInstallationRepositoryConfiguration CreateGitHubInstallationRepositoryConfiguration(
            int gitHubId,
            bool enableTracking) =>
            new()
            {
                GithubId = gitHubId,
                EnableTracking = enableTracking
            };

        public static Repository CreateGitHubClientRepository(
            int id,
            string name,
            string fullName) =>
            new()
            {
                Id = id,
                Name = name,
                FullName = fullName
            };

        public static GetInstallationRepositoriesResponse CreateGitHubInstallationRepositoriesResponse(
            int totalCount,
            IEnumerable<Repository> repositories) =>
            new()
            {
                TotalCount = totalCount,
                Repositories = repositories
            };
    }
}
