using Dapper;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Integration.Model;
using FlowJudge.Workspaces.Domain.Repository.Model;
using FlowJudge.Workspaces.Infrastructure.Repositories.Repositories.DbModels;
using FlowJudge.Workspaces.Infrastructure.Repositories.Repositories.Mappers;
using Cfg = FlowJudge.Workspaces.Infrastructure.WorkspacesContextConfiguration;

namespace FlowJudge.Workspaces.Infrastructure.Repositories.Repositories
{
    internal sealed class RepositoryRepository : DapperRepository, IRepositoryRepository
    {
        public RepositoryRepository(ISqlSession sqlSession) : base(sqlSession)
        {
        }

        public async Task AddRepositoryAsync(RepositoryRoot repository, CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var model = repository.ToDbModel();

            var command = Command(AddRepositorySql, model, ct);
            await Connection.ExecuteAsync(command);
        }

        public async Task<IEnumerable<RepositoryRoot>> GetRepositoriesByIntegrationAsync(
            IntegrationId integrationId,
            CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var command = Command(GetRepositoriesByIntegrationSql, new { IntegrationId = integrationId.Value }, ct);
            var results = await Connection.QueryAsync<RepositoryDbModel>(command);

            return results.Select(r => r.ToDomain());
        }

        public async Task UpdateRepositoryAsync(RepositoryRoot repository, CancellationToken ct = default)
        {
            await EnsureConnectionOpenAsync(ct);
            var command = Command(UpdateRepositorySql, repository.ToDbModel(), ct);
            await Connection.ExecuteAsync(command);
        }

        private const string AddRepositorySql = @$"
INSERT INTO {Cfg.SchemaName}.{Cfg.RepositoriesTableName} (
     {nameof(RepositoryDbModel.id)}
    ,{nameof(RepositoryDbModel.aggregate_id)}
    ,{nameof(RepositoryDbModel.workspace_id)}
    ,{nameof(RepositoryDbModel.integration_id)}
    ,{nameof(RepositoryDbModel.vcs_external_id)}
    ,{nameof(RepositoryDbModel.name)}
    ,{nameof(RepositoryDbModel.full_name)}
    ,{nameof(RepositoryDbModel.is_tracking)}
    ,{nameof(RepositoryDbModel.status)}
) VALUES (
     @{nameof(RepositoryDbModel.id)}
    ,@{nameof(RepositoryDbModel.aggregate_id)}
    ,@{nameof(RepositoryDbModel.workspace_id)}
    ,@{nameof(RepositoryDbModel.integration_id)}
    ,@{nameof(RepositoryDbModel.vcs_external_id)}
    ,@{nameof(RepositoryDbModel.name)}
    ,@{nameof(RepositoryDbModel.full_name)}
    ,@{nameof(RepositoryDbModel.is_tracking)}
    ,@{nameof(RepositoryDbModel.status)}
)";

        private const string GetRepositoriesByIntegrationSql = @$"
SELECT
     {nameof(RepositoryDbModel.id)}
    ,{nameof(RepositoryDbModel.aggregate_id)}
    ,{nameof(RepositoryDbModel.workspace_id)}
    ,{nameof(RepositoryDbModel.integration_id)}
    ,{nameof(RepositoryDbModel.vcs_external_id)}
    ,{nameof(RepositoryDbModel.name)}
    ,{nameof(RepositoryDbModel.full_name)}
    ,{nameof(RepositoryDbModel.is_tracking)}
    ,{nameof(RepositoryDbModel.status)}
FROM {Cfg.SchemaName}.{Cfg.RepositoriesTableName}
WHERE {nameof(RepositoryDbModel.integration_id)} = @IntegrationId";

        private const string UpdateRepositorySql = $@"
UPDATE {Cfg.SchemaName}.{Cfg.RepositoriesTableName}
SET
     {nameof(RepositoryDbModel.name)} = @{nameof(RepositoryDbModel.name)}
    ,{nameof(RepositoryDbModel.full_name)} = @{nameof(RepositoryDbModel.full_name)}
    ,{nameof(RepositoryDbModel.is_tracking)} = @{nameof(RepositoryDbModel.is_tracking)}
    ,{nameof(RepositoryDbModel.status)} = @{nameof(RepositoryDbModel.status)}
WHERE {nameof(RepositoryDbModel.id)} = @{nameof(RepositoryDbModel.id)}";
    }
}
