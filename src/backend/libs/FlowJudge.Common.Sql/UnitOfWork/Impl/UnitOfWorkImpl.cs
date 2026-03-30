namespace FlowJudge.Common.Sql.UnitOfWork.Impl
{
    internal sealed class UnitOfWorkImpl(
        ISqlSession sqlSession)
            : IUnitOfWork
    {
        public Task BeginTransactionAsync(CancellationToken cancellationToken = default) =>
            sqlSession.BeginTransactionAsync(cancellationToken);

        public Task CommitAsync(CancellationToken cancellationToken = default) =>
            sqlSession.CommitAsync(cancellationToken);

        public Task RollbackAsync(CancellationToken cancellationToken = default) =>
            sqlSession.RollbackAsync(cancellationToken);

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
