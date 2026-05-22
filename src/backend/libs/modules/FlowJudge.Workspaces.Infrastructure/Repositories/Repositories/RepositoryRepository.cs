using Dapper;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Workspaces.Application.Abstractions.Ports;
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
) VALUES (
     @{nameof(RepositoryDbModel.id)}
    ,@{nameof(RepositoryDbModel.aggregate_id)}
    ,@{nameof(RepositoryDbModel.workspace_id)}
    ,@{nameof(RepositoryDbModel.integration_id)}
    ,@{nameof(RepositoryDbModel.vcs_external_id)}
    ,@{nameof(RepositoryDbModel.name)}
    ,@{nameof(RepositoryDbModel.full_name)}
    ,@{nameof(RepositoryDbModel.is_tracking)}
)";
    }
}
