using Dapper;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Common.Utils.Serialization;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Infrastructure.Repositories.Integrations.DbModels;
using FlowJudge.Workspaces.Infrastructure.Repositories.Integrations.Mappers;
using Cfg = FlowJudge.Workspaces.Infrastructure.WorkspacesContextConfiguration;

namespace FlowJudge.Workspaces.Infrastructure.Repositories.Integrations
{
    internal sealed class IntegrationRepository : DapperRepository, IIntegrationRepository
    {
        public IntegrationRepository(ISqlSession sqlSession) : base(sqlSession)
        {
        }

        public async Task AddIntegrationAsync(IntegrationRoot integration, CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var dbModel = integration.ToDbModel();

            var insertIntegrationCmd = Command(AddIntegrationSql, dbModel.integration, ct);
            await Connection.ExecuteAsync(insertIntegrationCmd);

            if (dbModel.authenticationData.Any())
            {
                var insertMembersCmd = Command(AddIntegrationAuthenticationSql, dbModel.authenticationData, ct);
                await Connection.ExecuteAsync(insertMembersCmd);
            }
        }

        public async Task<IntegrationRoot?> GetIntegrationByAggregateIdAsync(IntegrationId integrationId, CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var getAggregateCommand = Command(GetIntegrationByAggregateIdSql, new { AggregateId = integrationId.Value }, ct);
            var integrationDbModel = await Connection.QuerySingleOrDefaultAsync<IntegrationDbModel>(getAggregateCommand);

            if (integrationDbModel is null)
                return null;

            var getAuthenticationCommand = Command(GetIntegrationAuthenticationByIntegrationIdSql, new { IntegrationId = integrationDbModel.id }, ct);
            var authenticationDbModels = await Connection.QueryAsync<IntegrationAuthenticationDbModel>(getAuthenticationCommand);

            return integrationDbModel.ToDomainModel(authenticationDbModels);
        }

        public async Task UpdateIntegrationAsync(IntegrationRoot integration, CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var dbModel = integration.ToDbModel();

            var updateIntegrationCmd = Command(UpdateIntegrationSql, dbModel.integration, ct);
            await Connection.ExecuteAsync(updateIntegrationCmd);

            var updateAuthenticationCmd = Command(
                UpdateIntegrationAuthenticationSql,
                new { AuthenticationData = dbModel.authenticationData.ToJson(), IntegrationId = integration.Id },
                ct);
            await Connection.ExecuteAsync(updateAuthenticationCmd);
        }

        public async Task<PagedList<IntegrationListItem>> GetIntegrationsAsync(
            WorkspaceId workspace,
            PageQuery pagination,
            CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var getCommand = Command(
                GetIntegrationsByWorkspaceIdSql,
                new
                {
                    WorkspaceId = workspace.Value,
                    Offset = (pagination.PageNumber - 1) * pagination.PageSize,
                    Limit = pagination.PageSize
                },
                ct);
            var integrationListItems = await Connection.QueryAsync<IntegrationListItemDbModel>(getCommand);
            var totalCount = await Connection.ExecuteScalarAsync<int>(Command(GetIntegrationsCountByWorkspaceIdSql, new { WorkspaceId = workspace.Value }, ct));

            return new PagedList<IntegrationListItem>(
                integrationListItems.Select(i => i.ToModel()).ToList(),
                pagination.PageNumber,
                pagination.PageSize,
                totalCount);
        }

        public async Task<IntegrationProvider?> GetIntegrationProviderAsync(IntegrationId integrationId, CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var command = Command(GetIntegrationProviderSql, new { AggregateId = integrationId.Value }, ct);
            var providerStr = await Connection.ExecuteScalarAsync<string?>(command);

            if (string.IsNullOrWhiteSpace(providerStr))
                return null;

            if (Enum.TryParse<IntegrationProvider>(providerStr, out var provider))
                return provider;

            return null;
        }


        private const string AddIntegrationSql = $@"
INSERT INTO {Cfg.SchemaName}.{Cfg.IntegrationsTableName} (
     {nameof(IntegrationDbModel.id)}
    ,{nameof(IntegrationDbModel.aggregate_id)}
    ,{nameof(IntegrationDbModel.workspace_id)}
    ,{nameof(IntegrationDbModel.name)}
    ,{nameof(IntegrationDbModel.provider)}
    ,{nameof(IntegrationDbModel.status)}
    ,{nameof(IntegrationDbModel.created_at)}
    ,{nameof(IntegrationDbModel.created_by)}
) 
VALUES (
     @{nameof(IntegrationDbModel.id)}
    ,@{nameof(IntegrationDbModel.aggregate_id)}
    ,@{nameof(IntegrationDbModel.workspace_id)}
    ,@{nameof(IntegrationDbModel.name)}
    ,@{nameof(IntegrationDbModel.provider)}
    ,@{nameof(IntegrationDbModel.status)}
    ,@{nameof(IntegrationDbModel.created_at)}
    ,@{nameof(IntegrationDbModel.created_by)}
);";

        private const string AddIntegrationAuthenticationSql = $@"
INSERT INTO {Cfg.SchemaName}.{Cfg.IntegrationAuthenticationTableName} (
     {nameof(IntegrationAuthenticationDbModel.id)}
    ,{nameof(IntegrationAuthenticationDbModel.integration_id)}
    ,{nameof(IntegrationAuthenticationDbModel.type)}
    ,{nameof(IntegrationAuthenticationDbModel.status)}
    ,{nameof(IntegrationAuthenticationDbModel.value)}
    ,{nameof(IntegrationAuthenticationDbModel.valid_to)}
    ,{nameof(IntegrationAuthenticationDbModel.created_at)}
    ,{nameof(IntegrationAuthenticationDbModel.created_by)}
)
VALUES (
     @{nameof(IntegrationAuthenticationDbModel.id)}
    ,@{nameof(IntegrationAuthenticationDbModel.integration_id)}
    ,@{nameof(IntegrationAuthenticationDbModel.type)}
    ,@{nameof(IntegrationAuthenticationDbModel.status)}
    ,@{nameof(IntegrationAuthenticationDbModel.value)}
    ,@{nameof(IntegrationAuthenticationDbModel.valid_to)}
    ,@{nameof(IntegrationAuthenticationDbModel.created_at)}
    ,@{nameof(IntegrationAuthenticationDbModel.created_by)}
);";

        private const string UpdateIntegrationSql = $@"
UPDATE {Cfg.SchemaName}.{Cfg.IntegrationsTableName} SET
     {nameof(IntegrationDbModel.name)} = @{nameof(IntegrationDbModel.name)}
    ,{nameof(IntegrationDbModel.status)} = @{nameof(IntegrationDbModel.status)}
WHERE {nameof(IntegrationDbModel.id)} = @{nameof(IntegrationDbModel.id)}";

        private const string UpdateIntegrationAuthenticationSql = $@"
WITH source AS (
    SELECT *
    FROM jsonb_to_recordset(CAST(@AuthenticationData AS jsonb)) AS x
    (
         {nameof(IntegrationAuthenticationDbModel.id)} uuid
        ,{nameof(IntegrationAuthenticationDbModel.integration_id)} uuid
        ,{nameof(IntegrationAuthenticationDbModel.type)} text
        ,{nameof(IntegrationAuthenticationDbModel.status)} text
        ,{nameof(IntegrationAuthenticationDbModel.value)} text
        ,{nameof(IntegrationAuthenticationDbModel.valid_to)} timestamptz
        ,{nameof(IntegrationAuthenticationDbModel.created_at)} timestamptz
        ,{nameof(IntegrationAuthenticationDbModel.created_by)} uuid
    )
),
upserted AS (
    INSERT INTO {Cfg.SchemaName}.{Cfg.IntegrationAuthenticationTableName} AS target
    (
         {nameof(IntegrationAuthenticationDbModel.id)}
        ,{nameof(IntegrationAuthenticationDbModel.integration_id)}
        ,{nameof(IntegrationAuthenticationDbModel.type)}
        ,{nameof(IntegrationAuthenticationDbModel.status)}
        ,{nameof(IntegrationAuthenticationDbModel.value)}
        ,{nameof(IntegrationAuthenticationDbModel.valid_to)}
        ,{nameof(IntegrationAuthenticationDbModel.created_at)}
        ,{nameof(IntegrationAuthenticationDbModel.created_by)}
    )
    SELECT
         source.{nameof(IntegrationAuthenticationDbModel.id)}
        ,source.{nameof(IntegrationAuthenticationDbModel.integration_id)}
        ,source.{nameof(IntegrationAuthenticationDbModel.type)}
        ,source.{nameof(IntegrationAuthenticationDbModel.status)}
        ,source.{nameof(IntegrationAuthenticationDbModel.value)}
        ,source.{nameof(IntegrationAuthenticationDbModel.valid_to)}
        ,source.{nameof(IntegrationAuthenticationDbModel.created_at)}
        ,source.{nameof(IntegrationAuthenticationDbModel.created_by)}
    FROM source
    ON CONFLICT ({nameof(IntegrationAuthenticationDbModel.id)}) DO UPDATE
    SET
         {nameof(IntegrationAuthenticationDbModel.integration_id)} = EXCLUDED.{nameof(IntegrationAuthenticationDbModel.integration_id)}
        ,{nameof(IntegrationAuthenticationDbModel.type)} = EXCLUDED.{nameof(IntegrationAuthenticationDbModel.type)}
        ,{nameof(IntegrationAuthenticationDbModel.status)} = EXCLUDED.{nameof(IntegrationAuthenticationDbModel.status)}
        ,{nameof(IntegrationAuthenticationDbModel.value)} = EXCLUDED.{nameof(IntegrationAuthenticationDbModel.value)}
        ,{nameof(IntegrationAuthenticationDbModel.valid_to)} = EXCLUDED.{nameof(IntegrationAuthenticationDbModel.valid_to)}
        ,{nameof(IntegrationAuthenticationDbModel.created_at)} = EXCLUDED.{nameof(IntegrationAuthenticationDbModel.created_at)}
        ,{nameof(IntegrationAuthenticationDbModel.created_by)} = EXCLUDED.{nameof(IntegrationAuthenticationDbModel.created_by)}
    RETURNING target.{nameof(IntegrationAuthenticationDbModel.id)}
)
DELETE FROM {Cfg.SchemaName}.{Cfg.IntegrationAuthenticationTableName} AS target
WHERE target.{nameof(IntegrationAuthenticationDbModel.integration_id)} = @IntegrationId
  AND NOT EXISTS (
      SELECT 1
      FROM source
      WHERE source.{nameof(IntegrationAuthenticationDbModel.id)} = target.{nameof(IntegrationAuthenticationDbModel.id)}
  );";

        private const string GetIntegrationByAggregateIdSql = $@"
SELECT
     {nameof(IntegrationDbModel.id)}
    ,{nameof(IntegrationDbModel.aggregate_id)}
    ,{nameof(IntegrationDbModel.workspace_id)}
    ,{nameof(IntegrationDbModel.name)}
    ,{nameof(IntegrationDbModel.provider)}
    ,{nameof(IntegrationDbModel.status)}
    ,{nameof(IntegrationDbModel.created_at)}
    ,{nameof(IntegrationDbModel.created_by)}
FROM {Cfg.SchemaName}.{Cfg.IntegrationsTableName}
WHERE {nameof(IntegrationDbModel.aggregate_id)} = @AggregateId";

        private const string GetIntegrationAuthenticationByIntegrationIdSql = $@"
SELECT
     {nameof(IntegrationAuthenticationDbModel.id)}
    ,{nameof(IntegrationAuthenticationDbModel.integration_id)}
    ,{nameof(IntegrationAuthenticationDbModel.type)}
    ,{nameof(IntegrationAuthenticationDbModel.status)}
    ,{nameof(IntegrationAuthenticationDbModel.value)}
    ,{nameof(IntegrationAuthenticationDbModel.valid_to)}
    ,{nameof(IntegrationAuthenticationDbModel.created_at)}
    ,{nameof(IntegrationAuthenticationDbModel.created_by)}
FROM {Cfg.SchemaName}.{Cfg.IntegrationAuthenticationTableName}
WHERE {nameof(IntegrationAuthenticationDbModel.integration_id)} = @IntegrationId";

        private const string GetIntegrationsCountByWorkspaceIdSql = $@"
SELECT COUNT(DISTINCT {nameof(IntegrationDbModel.id)})
FROM {Cfg.SchemaName}.{Cfg.IntegrationsTableName}
WHERE {nameof(IntegrationDbModel.workspace_id)} = @WorkspaceId";

        private const string GetIntegrationsByWorkspaceIdSql = $@"
SELECT 
     {nameof(IntegrationListItemDbModel.aggregate_id)}
    ,{nameof(IntegrationListItemDbModel.name)}
    ,{nameof(IntegrationListItemDbModel.provider)}
    ,{nameof(IntegrationListItemDbModel.status)}
    ,{nameof(IntegrationListItemDbModel.created_at)}
    ,{nameof(IntegrationListItemDbModel.created_by)}
FROM {Cfg.SchemaName}.{Cfg.IntegrationsTableName}
WHERE {nameof(IntegrationDbModel.workspace_id)} = @WorkspaceId
OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

        private const string GetIntegrationProviderSql = $@"
SELECT {nameof(IntegrationDbModel.provider)}
FROM {Cfg.SchemaName}.{Cfg.IntegrationsTableName}
WHERE {nameof(IntegrationDbModel.aggregate_id)} = @AggregateId";
    }
}
