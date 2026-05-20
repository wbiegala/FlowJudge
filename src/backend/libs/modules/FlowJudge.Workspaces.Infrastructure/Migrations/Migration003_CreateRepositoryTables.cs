using FlowJudge.Common.Sql.Migrations;
using FlowJudge.Workspaces.Infrastructure.Repositories.Repositories.DbModels;
using Cfg = FlowJudge.Workspaces.Infrastructure.WorkspacesContextConfiguration;

namespace FlowJudge.Workspaces.Infrastructure.Migrations
{
    [Migration(3, "Create Repository Tables")]
    internal class Migration003_CreateRepositoryTables : IMigration
    {
        public async Task ExecuteAsync(IMigrationContext migrationContext)
        {
            await migrationContext.ExecuteAsync(CreateTableSql);
        }

        private const string CreateTableSql = $@"
CREATE TABLE {Cfg.SchemaName}.{Cfg.RepositoriesTableName} (
    {nameof(RepositoryDbModel.id)} UUID PRIMARY KEY,
    {nameof(RepositoryDbModel.aggregate_id)} UUID NOT NULL,
    {nameof(RepositoryDbModel.workspace_id)} UUID NOT NULL,
    {nameof(RepositoryDbModel.integration_id)} UUID NOT NULL,
    {nameof(RepositoryDbModel.vcs_external_id)} TEXT NOT NULL,
    {nameof(RepositoryDbModel.name)} TEXT NOT NULL,
    {nameof(RepositoryDbModel.full_name)} TEXT,
    {nameof(RepositoryDbModel.is_tracking)} BOOLEAN NOT NULL
);

CREATE UNIQUE INDEX {Cfg.RepositoriesTableName}_unique_index
    ON {Cfg.SchemaName}.{Cfg.RepositoriesTableName} ({nameof(RepositoryDbModel.aggregate_id)});

CREATE INDEX {Cfg.RepositoriesTableName}_index
    ON {Cfg.SchemaName}.{Cfg.RepositoriesTableName} ({nameof(RepositoryDbModel.workspace_id)});";
    }
}
