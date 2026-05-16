using Dapper;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Common.Utils.Pagination;
using FlowJudge.Common.Utils.Serialization;
using FlowJudge.Workspaces.Application.Abstractions.Models;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Infrastructure.Repositories.Workspaces.DbModel;
using FlowJudge.Workspaces.Infrastructure.Repositories.Workspaces.Mappers;
using Cfg = FlowJudge.Workspaces.Infrastructure.WorkspacesContextConfiguration;

namespace FlowJudge.Workspaces.Infrastructure.Repositories.Workspaces
{
    internal sealed class WorkspaceRepository : DapperRepository, IWorkspaceRepository
    {
        public WorkspaceRepository(ISqlSession sqlSession) : base(sqlSession)
        {
        }

        public async Task<WorkspaceRoot?> GetWorkspaceByAggregateIdAsync(WorkspaceId workspaceId, CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var getAggregateCommand = Command(GetWorkspaceByAggregateIdSql, new { AggregateId = workspaceId.Value }, ct);
            var workspaceDbModel = await Connection.QuerySingleOrDefaultAsync<WorkspaceDbModel>(getAggregateCommand);

            if (workspaceDbModel is null)
                return null;

            var getMembersCommand = Command(GetWorkspaceMembersByWorkspaceIdSql, new { Id = workspaceDbModel.id }, ct);
            var workspaceMemberDbModels = await Connection.QueryAsync<WorkspaceMemberDbModel>(getMembersCommand);

            return workspaceDbModel.ToDomainModel(workspaceMemberDbModels);
        }

        public async Task AddWorkspaceAsync(WorkspaceRoot workspace, CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var dbModel = workspace.ToDbModel();

            var insertWorkspaceCmd = Command(AddWorkspaceSql, dbModel.workspace, ct);
            await Connection.ExecuteAsync(insertWorkspaceCmd);

            var insertMembersCmd = Command(AddWorkspaceMemberSql, dbModel.members, ct);
            await Connection.ExecuteAsync(insertMembersCmd);
        }

        public async Task UpdateWorkspaceAsync(WorkspaceRoot workspace, CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var dbModel = workspace.ToDbModel();

            var updateWorkspaceCmd = Command(UpdateWorkspaceSql, dbModel.workspace, ct);
            await Connection.ExecuteAsync(updateWorkspaceCmd);

            var updateMembersCmd = Command(UpdateWorkspaceMemberSql, new { Members = dbModel.members.ToJson(), WorkspaceId = dbModel.workspace.id }, ct);
            await Connection.ExecuteAsync(updateMembersCmd);
        }

        public async Task<PagedList<WorkspaceListItem>> GetUserWorkspacesAsync(Guid userId, PageQuery pagination, CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);

            var offset = (pagination.PageNumber - 1) * pagination.PageSize;

            var getCountCommand = Command(GetUserWorkspacesCountSql, new { UserId = userId }, ct);
            var totalCount = await Connection.QuerySingleAsync<int>(getCountCommand);

            var getItemsCommand = Command(
                GetUserWorkspacesSql,
                new
                {
                    UserId = userId,
                    Offset = offset,
                    Limit = pagination.PageSize
                },
                ct);
            var workspaceDbModels = await Connection.QueryAsync<WorkspaceListItemDbModel>(getItemsCommand);

            var items = workspaceDbModels.Select(x => x.ToModel()).ToList();

            return new PagedList<WorkspaceListItem>(items, pagination.PageSize, pagination.PageNumber, totalCount);
        }

        public async Task<WorkspaceRole?> GetUserRoleInWorkspaceAsync(
            WorkspaceId workspaceId,
            Guid userId,
            CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var getRoleCommand = Command(GetUserRoleInWorkspaceSql, new { AggregateId = workspaceId.Value, UserId = userId }, ct);
            var roleStr = await Connection.QuerySingleOrDefaultAsync<string>(getRoleCommand);

            if (!string.IsNullOrWhiteSpace(roleStr) && Enum.TryParse<WorkspaceRole>(roleStr, true, out var role))
                return role;

            return null;
        }

        private const string GetWorkspaceByAggregateIdSql = @$"
SELECT 
     {nameof(WorkspaceDbModel.id)}
    ,{nameof(WorkspaceDbModel.aggregate_id)}
    ,{nameof(WorkspaceDbModel.name)}
    ,{nameof(WorkspaceDbModel.status)}
    ,{nameof(WorkspaceDbModel.created_at)}
    ,{nameof(WorkspaceDbModel.created_by)}
FROM {Cfg.SchemaName}.{Cfg.WorkspacesTableName} workspace
WHERE {nameof(WorkspaceDbModel.aggregate_id)} = @AggregateId;";

        private const string GetWorkspaceMembersByWorkspaceIdSql = $@"
SELECT
     {nameof(WorkspaceMemberDbModel.id)}
    ,{nameof(WorkspaceMemberDbModel.workspace_id)}
    ,{nameof(WorkspaceMemberDbModel.member_id)}
    ,{nameof(WorkspaceMemberDbModel.role)}
    ,{nameof(WorkspaceMemberDbModel.assigned_at)}
    ,{nameof(WorkspaceMemberDbModel.assigned_by)}
FROM {Cfg.SchemaName}.{Cfg.WorkspaceMembersTableName} members
WHERE {nameof(WorkspaceMemberDbModel.workspace_id)} = @Id";

        private const string AddWorkspaceSql = $@"
INSERT INTO {Cfg.SchemaName}.{Cfg.WorkspacesTableName} (
     {nameof(WorkspaceDbModel.id)}
    ,{nameof(WorkspaceDbModel.aggregate_id)}
    ,{nameof(WorkspaceDbModel.name)}
    ,{nameof(WorkspaceDbModel.status)}
    ,{nameof(WorkspaceDbModel.created_at)}
    ,{nameof(WorkspaceDbModel.created_by)}
)
VALUES (
     @{nameof(WorkspaceDbModel.id)}
    ,@{nameof(WorkspaceDbModel.aggregate_id)}
    ,@{nameof(WorkspaceDbModel.name)}
    ,@{nameof(WorkspaceDbModel.status)}
    ,@{nameof(WorkspaceDbModel.created_at)}
    ,@{nameof(WorkspaceDbModel.created_by)}
);";

        private const string AddWorkspaceMemberSql = $@"
INSERT INTO {Cfg.SchemaName}.{Cfg.WorkspaceMembersTableName} (
     {nameof(WorkspaceMemberDbModel.id)}
    ,{nameof(WorkspaceMemberDbModel.workspace_id)}
    ,{nameof(WorkspaceMemberDbModel.member_id)}
    ,{nameof(WorkspaceMemberDbModel.role)}
    ,{nameof(WorkspaceMemberDbModel.assigned_at)}
    ,{nameof(WorkspaceMemberDbModel.assigned_by)}
) 
VALUES (
     @{nameof(WorkspaceMemberDbModel.id)}
    ,@{nameof(WorkspaceMemberDbModel.workspace_id)}
    ,@{nameof(WorkspaceMemberDbModel.member_id)}
    ,@{nameof(WorkspaceMemberDbModel.role)}
    ,@{nameof(WorkspaceMemberDbModel.assigned_at)}
    ,@{nameof(WorkspaceMemberDbModel.assigned_by)}
);";

        private const string UpdateWorkspaceSql = $@"
UPDATE {Cfg.SchemaName}.{Cfg.WorkspacesTableName}
SET
     {nameof(WorkspaceDbModel.name)} = @{nameof(WorkspaceDbModel.name)}
    ,{nameof(WorkspaceDbModel.status)} = @{nameof(WorkspaceDbModel.status)}
WHERE {nameof(WorkspaceDbModel.aggregate_id)} = @{nameof(WorkspaceDbModel.aggregate_id)}
";

        private const string UpdateWorkspaceMemberSql = $@"
WITH source AS (
    SELECT
        x.{nameof(WorkspaceMemberDbModel.id)},
        x.{nameof(WorkspaceMemberDbModel.workspace_id)},
        x.{nameof(WorkspaceMemberDbModel.member_id)},
        x.{nameof(WorkspaceMemberDbModel.role)},
        x.{nameof(WorkspaceMemberDbModel.assigned_at)},
        x.{nameof(WorkspaceMemberDbModel.assigned_by)}
    FROM jsonb_to_recordset(CAST(@Members AS jsonb)) AS x(
        {nameof(WorkspaceMemberDbModel.id)} uuid,
        {nameof(WorkspaceMemberDbModel.workspace_id)} uuid,
        {nameof(WorkspaceMemberDbModel.member_id)} uuid,
        {nameof(WorkspaceMemberDbModel.role)} text,
        {nameof(WorkspaceMemberDbModel.assigned_at)} timestamptz,
        {nameof(WorkspaceMemberDbModel.assigned_by)} uuid
    )
),
upserted AS (
    INSERT INTO {Cfg.SchemaName}.{Cfg.WorkspaceMembersTableName} (
        {nameof(WorkspaceMemberDbModel.id)},
        {nameof(WorkspaceMemberDbModel.workspace_id)},
        {nameof(WorkspaceMemberDbModel.member_id)},
        {nameof(WorkspaceMemberDbModel.role)},
        {nameof(WorkspaceMemberDbModel.assigned_at)},
        {nameof(WorkspaceMemberDbModel.assigned_by)}
    )
    SELECT
        s.{nameof(WorkspaceMemberDbModel.id)},
        s.{nameof(WorkspaceMemberDbModel.workspace_id)},
        s.{nameof(WorkspaceMemberDbModel.member_id)},
        s.{nameof(WorkspaceMemberDbModel.role)},
        s.{nameof(WorkspaceMemberDbModel.assigned_at)},
        s.{nameof(WorkspaceMemberDbModel.assigned_by)}
    FROM source s
    ON CONFLICT ({nameof(WorkspaceMemberDbModel.id)}) DO UPDATE
    SET
        {nameof(WorkspaceMemberDbModel.workspace_id)} = EXCLUDED.{nameof(WorkspaceMemberDbModel.workspace_id)},
        {nameof(WorkspaceMemberDbModel.member_id)} = EXCLUDED.{nameof(WorkspaceMemberDbModel.member_id)},
        {nameof(WorkspaceMemberDbModel.role)} = EXCLUDED.{nameof(WorkspaceMemberDbModel.role)},
        {nameof(WorkspaceMemberDbModel.assigned_at)} = EXCLUDED.{nameof(WorkspaceMemberDbModel.assigned_at)},
        {nameof(WorkspaceMemberDbModel.assigned_by)} = EXCLUDED.{nameof(WorkspaceMemberDbModel.assigned_by)}
    RETURNING {nameof(WorkspaceMemberDbModel.id)}
)
DELETE FROM {Cfg.SchemaName}.{Cfg.WorkspaceMembersTableName} target
WHERE target.{nameof(WorkspaceMemberDbModel.workspace_id)} = @WorkspaceId
  AND NOT EXISTS (
      SELECT 1
      FROM source s
      WHERE s.id = target.{nameof(WorkspaceMemberDbModel.id)}
  );";

        private const string GetUserWorkspacesCountSql = $@"
SELECT COUNT(DISTINCT w.{nameof(WorkspaceDbModel.id)})
FROM {Cfg.SchemaName}.{Cfg.WorkspacesTableName} w
INNER JOIN {Cfg.SchemaName}.{Cfg.WorkspaceMembersTableName} wm 
    ON wm.{nameof(WorkspaceMemberDbModel.workspace_id)} = w.{nameof(WorkspaceDbModel.id)}
WHERE wm.{nameof(WorkspaceMemberDbModel.member_id)} = @UserId";

        private const string GetUserWorkspacesSql = $@"
SELECT 
     w.{nameof(WorkspaceDbModel.aggregate_id)} as {nameof(WorkspaceListItemDbModel.workspace_id)}
    ,w.{nameof(WorkspaceDbModel.name)} as {nameof(WorkspaceListItemDbModel.name)}
    ,(SELECT {nameof(WorkspaceMemberDbModel.member_id)}
      FROM {Cfg.SchemaName}.{Cfg.WorkspaceMembersTableName}
      WHERE {nameof(WorkspaceMemberDbModel.workspace_id)} = w.{nameof(WorkspaceDbModel.id)}
        AND {nameof(WorkspaceMemberDbModel.role)} = 'Owner'
      LIMIT 1) as {nameof(WorkspaceListItemDbModel.owner_id)}
    ,wm.{nameof(WorkspaceMemberDbModel.role)} as {nameof(WorkspaceListItemDbModel.role)}
    ,w.{nameof(WorkspaceDbModel.created_at)} as {nameof(WorkspaceListItemDbModel.created_at)}
FROM {Cfg.SchemaName}.{Cfg.WorkspacesTableName} w
INNER JOIN {Cfg.SchemaName}.{Cfg.WorkspaceMembersTableName} wm 
    ON wm.{nameof(WorkspaceMemberDbModel.workspace_id)} = w.{nameof(WorkspaceDbModel.id)}
WHERE wm.{nameof(WorkspaceMemberDbModel.member_id)} = @UserId
OFFSET @Offset ROWS FETCH NEXT @Limit ROWS ONLY";

        private const string GetUserRoleInWorkspaceSql = $@"
SELECT DISTINCT {nameof(WorkspaceMemberDbModel.role)}
FROM {Cfg.SchemaName}.{Cfg.WorkspacesTableName} w
LEFT JOIN {Cfg.SchemaName}.{Cfg.WorkspaceMembersTableName} wm
    ON wm.{nameof(WorkspaceMemberDbModel.workspace_id)} = w.{nameof(WorkspaceDbModel.id)}
WHERE w.{nameof(WorkspaceDbModel.aggregate_id)} = @AggregateId
    AND wm.{nameof(WorkspaceMemberDbModel.member_id)} = @UserId";
    }
}
