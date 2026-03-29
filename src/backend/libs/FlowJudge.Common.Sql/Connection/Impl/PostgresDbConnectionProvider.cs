using System.Data;

namespace FlowJudge.Common.Sql.Connection.Impl
{
    internal sealed class PostgresDbConnectionProvider : IDbConnectionProvider
    {
        private readonly string _connectionString;

        public PostgresDbConnectionProvider(string connectionString)
        {
            _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public IDbConnection GetDbConnection()
        {
            return new Npgsql.NpgsqlConnection(_connectionString);
        }
    }
}
