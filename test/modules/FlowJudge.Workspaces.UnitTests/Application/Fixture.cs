using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Workspaces.Application.Abstractions.Commands;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Queries;
using FlowJudge.Workspaces.Domain.Integration.Model;

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
    }
}
