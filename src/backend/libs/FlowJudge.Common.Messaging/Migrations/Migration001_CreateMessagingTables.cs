using FlowJudge.Common.Messaging.Outbox;
using FlowJudge.Common.Sql.Migrations;

namespace FlowJudge.Common.Messaging.Migrations
{
    [Migration(1, "Create messaging tables")]
    internal class Migration001_CreateMessagingTables : IMigration
    {
        public async Task ExecuteAsync(IMigrationContext migrationContext)
        {
            await migrationContext.ExecuteAsync(EnsureSchemaExistsSql);
            await migrationContext.ExecuteAsync(CreateOutboxMessageTableSql);
            await migrationContext.ExecuteAsync(CreateOutboxMessageLogTableSql);
        }

        private const string EnsureSchemaExistsSql = $@"
create schema if not exists {OutboxConfiguration.SchemaName};
";

        private const string CreateOutboxMessageTableSql = $@"
CREATE TABLE {OutboxConfiguration.SchemaName}.{OutboxConfiguration.OutboxMessageTableName} (
    id UUID PRIMARY KEY,
    type TEXT NOT NULL,
    system_id UUID NOT NULL,
    payload BYTEA NOT NULL,
    publication_timestamp TIMESTAMPTZ NOT NULL
);";

        private const string CreateOutboxMessageLogTableSql = $@"
CREATE TABLE {OutboxConfiguration.SchemaName}.{OutboxConfiguration.OutboxMessageLogTableName} (
    id UUID PRIMARY KEY,
    outbox_message_id UUID NOT NULL,
    occured_timestamp TIMESTAMPTZ NOT NULL,
    CONSTRAINT fk_outbox_message
        FOREIGN KEY(outbox_message_id) 
        REFERENCES {OutboxConfiguration.SchemaName}.{OutboxConfiguration.OutboxMessageTableName}(id)
        ON DELETE CASCADE
);";
    }
}
