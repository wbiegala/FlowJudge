using FlowJudge.Common.Sql.Migrations;
using FlowJudge.Workspaces.Infrastructure.Repositories.Integrations.DbModels;
using Cfg = FlowJudge.Workspaces.Infrastructure.WorkspacesContextConfiguration;

namespace FlowJudge.Workspaces.Infrastructure.Migrations
{
    [Migration(2, "Create Integration Tables")]
    internal sealed class Migration002_CreateIntegrationTables : IMigration
    {
        public async Task ExecuteAsync(IMigrationContext migrationContext)
        {
            await migrationContext.ExecuteAsync(CreateIntegrationsTableSql);
            await migrationContext.ExecuteAsync(CreateIntegrationAuthenticationTableSql);
        }

        private const string CreateIntegrationsTableSql = @$"
CREATE TABLE {Cfg.SchemaName}.{Cfg.IntegrationsTableName} (
    {nameof(IntegrationDbModel.id)} UUID PRIMARY KEY,
    {nameof(IntegrationDbModel.aggregate_id)} UUID NOT NULL,
    {nameof(IntegrationDbModel.workspace_id)} UUID NOT NULL,
    {nameof(IntegrationDbModel.name)} TEXT NOT NULL,
    {nameof(IntegrationDbModel.provider)} TEXT NOT NULL,
    {nameof(IntegrationDbModel.status)} TEXT NOT NULL,
    {nameof(IntegrationDbModel.created_at)} TIMESTAMPTZ NOT NULL,
    {nameof(IntegrationDbModel.created_by)} UUID NOT NULL
);

CREATE UNIQUE INDEX {Cfg.IntegrationsTableName}_unique_index
    ON {Cfg.SchemaName}.{Cfg.IntegrationsTableName} ({nameof(IntegrationDbModel.aggregate_id)});";

        private const string CreateIntegrationAuthenticationTableSql = @$"
CREATE TABLE {Cfg.SchemaName}.{Cfg.IntegrationAuthenticationTableName} (
    {nameof(IntegrationAuthenticationDbModel.id)} UUID PRIMARY KEY,
    {nameof(IntegrationAuthenticationDbModel.integration_id)} UUID NOT NULL,
    {nameof(IntegrationAuthenticationDbModel.type)} TEXT NOT NULL,
    {nameof(IntegrationAuthenticationDbModel.status)} TEXT NOT NULL,
    {nameof(IntegrationAuthenticationDbModel.value)} TEXT NOT NULL,
    {nameof(IntegrationAuthenticationDbModel.valid_to)} TIMESTAMPTZ,
    {nameof(IntegrationAuthenticationDbModel.created_at)} TIMESTAMPTZ NOT NULL,
    {nameof(IntegrationAuthenticationDbModel.created_by)} UUID NOT NULL,
    CONSTRAINT fk_{Cfg.IntegrationAuthenticationTableName}_{Cfg.IntegrationsTableName}
        FOREIGN KEY ({nameof(IntegrationAuthenticationDbModel.integration_id)})
        REFERENCES {Cfg.SchemaName}.{Cfg.IntegrationsTableName} ({nameof(IntegrationDbModel.id)})
        ON DELETE CASCADE);";
    }
}
