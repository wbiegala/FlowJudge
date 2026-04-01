using FlowJudge.Common.Sql.Migrations;
using FlowJudge.Users.Infrastructure.Repositories.Users;
using Cfg = FlowJudge.Users.Infrastructure.UsersContextConfiguration;

namespace FlowJudge.Users.Infrastructure.Migrations
{
    [Migration(1, "Create User table")]
    internal sealed class Migration001_CreateUserTable : IMigration
    {
        public async Task ExecuteAsync(IMigrationContext migrationContext)
        {
            await migrationContext.ExecuteAsync(EnsureSchemaCreatedSql);
            await migrationContext.ExecuteAsync(CreateUserTableSql);
            await migrationContext.ExecuteAsync(CreateUserIdentityIdIndexSql);
        }

        private const string EnsureSchemaCreatedSql = $"create schema if not exists {Cfg.SchemaName};";

        private const string CreateUserTableSql = @$"
CREATE TABLE {Cfg.SchemaName}.{Cfg.UsersTableName} (
    {nameof(UserDbModel.id)} UUID PRIMARY KEY,
    {nameof(UserDbModel.identity_id)} UUID NOT NULL UNIQUE,
    {nameof(UserDbModel.username)} TEXT NOT NULL,
    {nameof(UserDbModel.email)} TEXT NOT NULL
);";

        private const string CreateUserIdentityIdIndexSql = @$"
create unique index if not exists ux_{Cfg.UsersTableName}_{nameof(UserDbModel.identity_id)}
    on {Cfg.SchemaName}.{Cfg.UsersTableName} ({nameof(UserDbModel.identity_id)});
";
    }
}
