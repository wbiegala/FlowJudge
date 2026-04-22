using Dapper;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Workspaces.Domain.Workspace.Model;
using FlowJudge.Workspaces.Infrastructure.Repositories.Workspaces.DbModel;
using FlowJudge.Workspaces.Infrastructure.Repositories.Workspaces.Mappers;

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
            var getAggregateCommand = Command(GetWorkspaceByAggregateIdSql, new { WorkspaceId = workspaceId.Value }, ct);
            var workspaceDbModel = await Connection.QuerySingleOrDefaultAsync<WorkspaceDbModel>(getAggregateCommand);

            if (workspaceDbModel is null)
                return null;

            var getMembersCommand = Command(GetWorkspaceMembersByAggregateIdSql, new { WorkspaceId = workspaceDbModel.id }, ct);
            var workspaceMemberDbModels = await Connection.QueryAsync<WorkspaceMemberDbModel>(getMembersCommand);

            return workspaceDbModel.ToDomainModel(workspaceMemberDbModels);
        }

        public Task AddWorkspaceAsync(WorkspaceRoot workspace, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task UpdateWorkspaceAsync(WorkspaceRoot workspace, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }


        private const string GetWorkspaceByAggregateIdSql = @$"
SELECT 
     {nameof(WorkspaceDbModel.id)}
    ,{nameof(WorkspaceDbModel.workspace_id)}
    ,{nameof(WorkspaceDbModel.name)}
    ,{nameof(WorkspaceDbModel.status)}
    ,{nameof(WorkspaceDbModel.created_at)}
    ,{nameof(WorkspaceDbModel.created_by)}
FROM {WorkspacesContextConfiguration.SchemaName}.{WorkspacesContextConfiguration.WorkspacesTableName} workspace
WHERE {nameof(WorkspaceDbModel.workspace_id)} = @WorkspaceId;";

        private const string GetWorkspaceMembersByAggregateIdSql = $@"
SELECT
     {nameof(WorkspaceMemberDbModel.id)}
    ,{nameof(WorkspaceMemberDbModel.workspace_id)}
    ,{nameof(WorkspaceMemberDbModel.member_id)}
    ,{nameof(WorkspaceMemberDbModel.role)}
    ,{nameof(WorkspaceMemberDbModel.assigned_at)}
    ,{nameof(WorkspaceMemberDbModel.assigned_by)}
FROM {WorkspacesContextConfiguration.SchemaName}.{WorkspacesContextConfiguration.WorkspaceMembersTableName} members
WHERE {nameof(WorkspaceMemberDbModel.workspace_id)} = @WorkspaceId";
    }
}
