using Dapper;
using System.Data;

namespace FlowJudge.Common.Sql.Migrations.Context
{
    internal sealed class MigrationContext(IDbConnection connection, IDbTransaction transaction) : IMigrationContext
    {
        public async Task ExecuteAsync(string sql, object? param = null, CancellationToken cancellationToken = default)
        {
            var command = new CommandDefinition(
                commandText: sql,
                parameters: param,
                transaction: transaction,
                cancellationToken: cancellationToken);
            await connection.ExecuteAsync(command);
        }

        public async Task<IReadOnlyList<T>> QueryAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default)
        {
            var command = new CommandDefinition(
                commandText: sql,
                parameters: param,
                transaction: transaction,
                cancellationToken: cancellationToken);
            var result = await connection.QueryAsync<T>(command);

            return result.AsList();
        }

        public async Task<T> QuerySingleAsync<T>(string sql, object? param = null, CancellationToken cancellationToken = default)
        {
            var command = new CommandDefinition(
                commandText: sql,
                parameters: param,
                transaction: transaction,
                cancellationToken: cancellationToken);

            return await connection.QuerySingleAsync<T>(command);
        }
    }
}
