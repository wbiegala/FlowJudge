using System.Data.Common;

namespace FlowJudge.Common.Sql.UnitOfWork
{
    public interface ISqlSession : IAsyncDisposable
    {
        DbConnection Connection { get; }
        DbTransaction? Transaction { get; }

        Task EnsureConnectionOpenAsync(CancellationToken cancellationToken);
        Task BeginTransactionAsync(CancellationToken cancellationToken);
        Task CommitAsync(CancellationToken cancellationToken);
        Task RollbackAsync(CancellationToken cancellationToken);
    }
}