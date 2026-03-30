using Dapper;
using System.Data.Common;

namespace FlowJudge.Common.Sql.UnitOfWork
{
    public abstract class DapperRepository(ISqlSession sqlSession)
    {
        protected DbConnection Connection => sqlSession.Connection;
        protected DbTransaction? Transaction => sqlSession.Transaction;

        protected Task EnsureConnectionOpenAsync(CancellationToken cancellationToken) =>
            sqlSession.EnsureConnectionOpenAsync(cancellationToken);

        protected CommandDefinition Command(
            string sql,
            object? parameters = null,
            CancellationToken cancellationToken = default) =>
            new(
                commandText: sql,
                parameters: parameters,
                transaction: Transaction,
                cancellationToken: cancellationToken);
    }
}
