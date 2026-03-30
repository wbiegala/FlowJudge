using FlowJudge.Common.Sql.Connection;
using System.Data;
using System.Data.Common;

namespace FlowJudge.Common.Sql.UnitOfWork.Impl
{
    internal sealed class SqlSession(
        IDbConnectionProvider dbConnectionProvider)
            : ISqlSession
    {
        private DbConnection? _connection;
        private DbTransaction? _transaction;

        public DbConnection Connection =>
            _connection ?? throw new InvalidOperationException("Database connection has not been initialized.");

        public DbTransaction? Transaction => _transaction;

        public async Task EnsureConnectionOpenAsync(CancellationToken cancellationToken)
        {
            if (_connection is null)
            {
                _connection = dbConnectionProvider.GetDbConnection();
            }

            if (_connection.State != ConnectionState.Open)
            {
                await _connection.OpenAsync(cancellationToken);
            }
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            if (_transaction is not null)
            {
                throw new InvalidOperationException("Database transaction has already been started.");
            }

            await EnsureConnectionOpenAsync(cancellationToken);
            _transaction = await _connection!.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            if (_transaction is null)
            {
                throw new InvalidOperationException("Database transaction has not been started.");
            }

            await _transaction.CommitAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async Task RollbackAsync(CancellationToken cancellationToken)
        {
            if (_transaction is null)
            {
                return;
            }

            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }

        public async ValueTask DisposeAsync()
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }

            if (_connection is not null)
            {
                await _connection.DisposeAsync();
                _connection = null;
            }
        }
    }
}
