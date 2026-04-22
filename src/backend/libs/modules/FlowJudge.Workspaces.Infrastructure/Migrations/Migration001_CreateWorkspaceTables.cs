using FlowJudge.Common.Sql.Migrations;
using FlowJudge.Workspaces.Infrastructure.Repositories.Workspaces.DbModel;
using Cfg = FlowJudge.Workspaces.Infrastructure.WorkspacesContextConfiguration;

namespace FlowJudge.Workspaces.Infrastructure.Migrations
{
    [Migration(1, "Create Workspace Tables")]
    internal sealed class Migration001_CreateWorkspaceTables : IMigration
    {
        public async Task ExecuteAsync(IMigrationContext migrationContext)
        {
            await migrationContext.ExecuteAsync(EnsureSchemaCreatedSql);
            await migrationContext.ExecuteAsync(CreateWorkspacesTableSql);
            await migrationContext.ExecuteAsync(CreateWorkspaceMembersTableSql);
        }

        private const string EnsureSchemaCreatedSql = $"create schema if not exists {Cfg.SchemaName};";

        private const string CreateWorkspacesTableSql = @$"
CREATE TABLE {Cfg.SchemaName}.{Cfg.WorkspacesTableName} (
    {nameof(WorkspaceDbModel.id)} UUID PRIMARY KEY,
    {nameof(WorkspaceDbModel.workspace_id)} UUID NOT NULL,
    {nameof(WorkspaceDbModel.name)} TEXT NOT NULL,
    {nameof(WorkspaceDbModel.status)} TEXT NOT NULL,
    {nameof(WorkspaceDbModel.created_at)} TIMESTAMPTZ NOT NULL,
    {nameof(WorkspaceDbModel.created_by)} UUID NOT NULL
);

create unique index if not exists ux_{Cfg.WorkspacesTableName}_{nameof(WorkspaceDbModel.workspace_id)}
    on {Cfg.SchemaName}.{Cfg.WorkspacesTableName} ({nameof(WorkspaceDbModel.workspace_id)});";

        private const string CreateWorkspaceMembersTableSql = @$"
CREATE TABLE {Cfg.SchemaName}.{Cfg.WorkspaceMembersTableName} (
    {nameof(WorkspaceMemberDbModel.id)} UUID PRIMARY KEY,
    {nameof(WorkspaceMemberDbModel.workspace_id)} UUID NOT NULL,
    {nameof(WorkspaceMemberDbModel.member_id)} UUID NOT NULL,
    {nameof(WorkspaceMemberDbModel.role)} TEXT NOT NULL,
    {nameof(WorkspaceMemberDbModel.assigned_at)} TIMESTAMPTZ NOT NULL,
    {nameof(WorkspaceMemberDbModel.assigned_by)} UUID,
    CONSTRAINT fk_{Cfg.WorkspaceMembersTableName}_{Cfg.WorkspacesTableName}
        FOREIGN KEY ({nameof(WorkspaceMemberDbModel.workspace_id)})
        REFERENCES {Cfg.SchemaName}.{Cfg.WorkspacesTableName} ({nameof(WorkspaceDbModel.id)})
        ON DELETE CASCADE);

create unique index if not exists ux_{Cfg.WorkspaceMembersTableName}_{nameof(WorkspaceMemberDbModel.workspace_id)}_{nameof(WorkspaceMemberDbModel.member_id)}
    on {Cfg.SchemaName}.{Cfg.WorkspaceMembersTableName} (
        {nameof(WorkspaceMemberDbModel.workspace_id)},
        {nameof(WorkspaceMemberDbModel.member_id)}
    );";
    }
}
