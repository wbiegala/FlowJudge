using Dapper;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Repository.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Infrastructure.Repositories.Integrations.DbModels;
using FlowJudge.Workspaces.Infrastructure.Repositories.Repositories.DbModels;
using FlowJudge.Workspaces.Infrastructure.Repositories.Repositories.Mappers;
using Cfg = FlowJudge.Workspaces.Infrastructure.WorkspacesContextConfiguration;

namespace FlowJudge.Workspaces.Infrastructure.Repositories.Repositories
{
    internal sealed class RepositoryRepository : DapperRepository, IRepositoryRepository
    {
        public RepositoryRepository(ISqlSession sqlSession) : base(sqlSession)
        {
        }

        public async Task AddRepositoryAsync(RepositoryRoot repository, CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var model = repository.ToDbModel();

            var command = Command(AddRepositorySql, model, ct);
            await Connection.ExecuteAsync(command);
        }

        public async Task<IEnumerable<RepositoryRoot>> GetRepositoriesByIntegrationAsync(
            IntegrationId integrationId,
            CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var command = Command(GetRepositoriesByIntegrationSql, new { IntegrationId = integrationId.Value }, ct);
            var results = await Connection.QueryAsync<RepositoryDbModel>(command);

            return results.Select(r => r.ToDomain());
        }

        public async Task UpdateRepositoryAsync(RepositoryRoot repository, CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var command = Command(UpdateRepositorySql, repository.ToDbModel(), ct);
            await Connection.ExecuteAsync(command);
        }

        public async Task<PagedList<RepositoryListItem>> GetRepositoriesAsync(
            WorkspaceId workspace,
            PageQuery pagination,
            CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var getCommand = Command(
                GetRepositoriesByWorkspaceIdSql,
                new
                {
                    WorkspaceId = workspace.Value,
                    Offset = (pagination.PageNumber - 1) * pagination.PageSize,
                    Limit = pagination.PageSize
                },
                ct);
            var results = await Connection.QueryAsync<RepositoryDbModel>(getCommand);
            var totalCount = await Connection.ExecuteScalarAsync<int>(Command(GetRepositoriesCountByWorkspaceIdSql, new { WorkspaceId = workspace.Value }, ct));

            return new PagedList<RepositoryListItem>(
                results.Select(rm => rm.ToListItem()).ToList(),
                pagination.PageSize,
                pagination.PageNumber,
                totalCount);
        }

        private const string AddRepositorySql = @$"
INSERT INTO {Cfg.SchemaName}.{Cfg.RepositoriesTableName} (
     {nameof(RepositoryDbModel.id)}
    ,{nameof(RepositoryDbModel.aggregate_id)}
    ,{nameof(RepositoryDbModel.workspace_id)}
    ,{nameof(RepositoryDbModel.integration_id)}
    ,{nameof(RepositoryDbModel.vcs_external_id)}
    ,{nameof(RepositoryDbModel.name)}
    ,{nameof(RepositoryDbModel.full_name)}
    ,{nameof(RepositoryDbModel.is_tracking)}
    ,{nameof(RepositoryDbModel.status)}
) VALUES (
     @{nameof(RepositoryDbModel.id)}
    ,@{nameof(RepositoryDbModel.aggregate_id)}
    ,@{nameof(RepositoryDbModel.workspace_id)}
    ,@{nameof(RepositoryDbModel.integration_id)}
    ,@{nameof(RepositoryDbModel.vcs_external_id)}
    ,@{nameof(RepositoryDbModel.name)}
    ,@{nameof(RepositoryDbModel.full_name)}
    ,@{nameof(RepositoryDbModel.is_tracking)}
    ,@{nameof(RepositoryDbModel.status)}
)";

        private const string GetRepositoriesByIntegrationSql = @$"
SELECT
     {nameof(RepositoryDbModel.id)}
    ,{nameof(RepositoryDbModel.aggregate_id)}
    ,{nameof(RepositoryDbModel.workspace_id)}
    ,{nameof(RepositoryDbModel.integration_id)}
    ,{nameof(RepositoryDbModel.vcs_external_id)}
    ,{nameof(RepositoryDbModel.name)}
    ,{nameof(RepositoryDbModel.full_name)}
    ,{nameof(RepositoryDbModel.is_tracking)}
    ,{nameof(RepositoryDbModel.status)}
FROM {Cfg.SchemaName}.{Cfg.RepositoriesTableName}
WHERE {nameof(RepositoryDbModel.integration_id)} = @IntegrationId
    AND {nameof(RepositoryDbModel.status)} <> 'Deleted'";

        private const string UpdateRepositorySql = $@"
UPDATE {Cfg.SchemaName}.{Cfg.RepositoriesTableName}
SET
     {nameof(RepositoryDbModel.name)} = @{nameof(RepositoryDbModel.name)}
    ,{nameof(RepositoryDbModel.full_name)} = @{nameof(RepositoryDbModel.full_name)}
    ,{nameof(RepositoryDbModel.is_tracking)} = @{nameof(RepositoryDbModel.is_tracking)}
    ,{nameof(RepositoryDbModel.status)} = @{nameof(RepositoryDbModel.status)}
WHERE {nameof(RepositoryDbModel.id)} = @{nameof(RepositoryDbModel.id)}";

        private const string GetRepositoriesByWorkspaceIdSql = @$"
SELECT
     {nameof(RepositoryDbModel.id)}
    ,{nameof(RepositoryDbModel.aggregate_id)}
    ,{nameof(RepositoryDbModel.workspace_id)}
    ,{nameof(RepositoryDbModel.integration_id)}
    ,{nameof(RepositoryDbModel.vcs_external_id)}
    ,{nameof(RepositoryDbModel.name)}
    ,{nameof(RepositoryDbModel.full_name)}
    ,{nameof(RepositoryDbModel.is_tracking)}
    ,{nameof(RepositoryDbModel.status)}
FROM {Cfg.SchemaName}.{Cfg.RepositoriesTableName}
WHERE {nameof(RepositoryDbModel.workspace_id)} = @WorkspaceId
    AND {nameof(RepositoryDbModel.status)} <> 'Deleted'
OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

        private const string GetRepositoriesCountByWorkspaceIdSql = $@"
SELECT COUNT(DISTINCT {nameof(RepositoryDbModel.id)})
FROM {Cfg.SchemaName}.{Cfg.RepositoriesTableName}
WHERE {nameof(RepositoryDbModel.workspace_id)} = @WorkspaceId
    AND {nameof(RepositoryDbModel.status)} <> 'Deleted'";

    }
}
