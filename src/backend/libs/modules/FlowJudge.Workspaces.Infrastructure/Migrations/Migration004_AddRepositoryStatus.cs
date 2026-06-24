using FlowJudge.Common.Sql.Migrations;
using FlowJudge.Workspaces.Infrastructure.Repositories.Repositories.DbModels;
using Cfg = FlowJudge.Workspaces.Infrastructure.WorkspacesContextConfiguration;

namespace FlowJudge.Workspaces.Infrastructure.Migrations
{
    [Migration(4, "Add Respository status column")]
    internal sealed class Migration004_AddRepositoryStatus : IMigration
    {
        public async Task ExecuteAsync(IMigrationContext migrationContext)
        {
            await migrationContext.ExecuteAsync(AddRepositoryStatusColumnSql);
        }

        private const string AddRepositoryStatusColumnSql = $@"
ALTER TABLE {Cfg.SchemaName}.{Cfg.RepositoriesTableName}
ADD COLUMN {nameof(RepositoryDbModel.status)} TEXT NOT NULL DEFAULT 'Active'";
    }
}
