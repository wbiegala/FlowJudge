using Microsoft.Extensions.Logging;
using System.Data.Common;
using Dapper;
using FlowJudge.Common.Sql.Connection;
using FlowJudge.Common.Sql.Migrations.Context;
using FlowJudge.Common.Utils.Time;

namespace FlowJudge.Common.Sql.Migrations.Executors
{
    internal sealed class PostgresMigrationExecutor(
        IEnumerable<IMigration> migrations,
        IDbConnectionProvider connectionProvider,
        ITimeService timeService,
        ILogger<PostgresMigrationExecutor> logger)
            : IMigrationExecutor
    {
        private const string MigrationSchemaName = "public";
        private const string MigrationTableName = "migrations";

        public async Task ExecuteAsync(CancellationToken ct = default)
        {
            await EnsureMigrationTableExistsAsync();

            var migrations = BuildMigrationExecutionContexts();
            var migrationByAssemblies = migrations.GroupBy(m => m.Assembly);
            await using var connection = connectionProvider.GetDbConnection();
            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync(ct);
            }

            foreach (var migrationAssembly in migrationByAssemblies)
            {
                var migrationsToExecute = migrationAssembly.OrderBy(mec => mec.Number).ToList();

                foreach (var migration in migrationsToExecute)
                {
                    if (ct.IsCancellationRequested)
                    {
                        logger.LogInformation("Migration execution cancelled, stopping");
                        return;
                    }

                    await ExecuteMigrationAsync(migration, connection);
                }
            }
        }

        private IEnumerable<MigrationExecutionContext> BuildMigrationExecutionContexts()
        {
            var result = new List<MigrationExecutionContext>();

            foreach (var migration in migrations)
            {
                var migrationType = migration.GetType();
                var attribute = migrationType.GetCustomAttributes(typeof(MigrationAttribute), false).FirstOrDefault() as MigrationAttribute;

                if (attribute is null)
                {
                    logger.LogWarning("Migration {migrationName} skipped bacause no metadata was included", migrationType.FullName);
                    continue;
                }

                result.Add(new MigrationExecutionContext
                {
                    Assembly = migrationType.Assembly.GetName().Name ?? string.Empty,
                    Number = attribute.Number,
                    Description = attribute.Description,
                    Migration = migration
                });
            }

            return result;
        }

        private async Task ExecuteMigrationAsync(MigrationExecutionContext migration, DbConnection connection)
        {
            await using var transaction = await connection.BeginTransactionAsync();

            try
            {
                var existingMigration = await connection.QueryFirstOrDefaultAsync<MigrationNotification>(GetMigrationByAssemblyAndNumberSql, new
                {
                    assembly = migration.Assembly,
                    number = migration.Number
                }, transaction);

                if (existingMigration is not null)
                {
                    await transaction.RollbackAsync();
                    logger.LogInformation("Migration {assembly} #{number} already executed at {timestamp}, skipping",
                        existingMigration.Assembly, existingMigration.Number, existingMigration.Timestamp);

                    return;
                }

                var migrationContext = new MigrationContext(connection, transaction);
                await migration.Migration.ExecuteAsync(migrationContext);

                var notification = new MigrationNotification
                {
                    Id = Guid.NewGuid(),
                    Assembly = migration.Assembly,
                    Number = migration.Number,
                    Description = migration.Description,
                    Timestamp = timeService.UtcNow
                };
                await connection.ExecuteAsync(InsertMigrationNotificationSql, notification, transaction);

                await transaction.CommitAsync();

                logger.LogInformation("Migration {assembly} #{number} executed at {timestamp}", notification.Assembly, notification.Number, notification.Timestamp);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Migration {assembly} #{number} failed with error: {errorMessage}", migration.Assembly, migration.Number, ex.Message);
                await transaction.RollbackAsync();

                throw;
            }
        }

        private async Task EnsureMigrationTableExistsAsync()
        {
            await using var connection = connectionProvider.GetDbConnection();

            if (connection.State != System.Data.ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            const string sql = $@"
create schema if not exists {MigrationSchemaName};

create table if not exists {MigrationSchemaName}.{MigrationTableName}
(
    id uuid primary key,
    assembly text not null,
    number integer not null check (number > 0),
    description text not null,
    execution_timestamp timestamptz not null
);

create unique index if not exists ux_migrations_number_assembly
    on {MigrationSchemaName}.{MigrationTableName} (assembly, number);";

            await connection.ExecuteAsync(sql);
        }

        private sealed class MigrationExecutionContext
        {
            public required string Assembly { get; init; }
            public int Number { get; init; }
            public required string Description { get; init; }
            public required IMigration Migration { get; init; }
        }

        private sealed record MigrationNotification
        {
            public Guid Id { get; init; }
            public required string Assembly { get; init; }
            public int Number { get; init; }
            public required string Description { get; init; }
            public required DateTimeOffset Timestamp { get; init; }
        }

        private const string GetMigrationByAssemblyAndNumberSql = $@"
SELECT 
     id                     {nameof(MigrationNotification.Id)}
    ,assembly               {nameof(MigrationNotification.Assembly)}
    ,number                 {nameof(MigrationNotification.Number)}
    ,description            {nameof(MigrationNotification.Description)}
    ,execution_timestamp    {nameof(MigrationNotification.Timestamp)}
FROM {MigrationSchemaName}.{MigrationTableName}
WHERE 
    {nameof(MigrationNotification.Assembly)} = @assembly
    AND {nameof(MigrationNotification.Number)} = @number";

        private const string InsertMigrationNotificationSql = $@"
INSERT INTO {MigrationSchemaName}.{MigrationTableName} (
     id
    ,assembly
    ,number
    ,description
    ,execution_timestamp
) VALUES (
     @{nameof(MigrationNotification.Id)}
    ,@{nameof(MigrationNotification.Assembly)}
    ,@{nameof(MigrationNotification.Number)}
    ,@{nameof(MigrationNotification.Description)}
    ,@{nameof(MigrationNotification.Timestamp)}
);";
    }
}
