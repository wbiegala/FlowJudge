namespace FlowJudge.Common.Sql.Migrations
{
    public interface IMigrationContext
    {
        Task ExecuteAsync(
            string sql,
            object? param = null,
            CancellationToken cancellationToken = default);

        Task<T> QuerySingleAsync<T>(
            string sql,
            object? param = null,
            CancellationToken cancellationToken = default);

        Task<IReadOnlyList<T>> QueryAsync<T>(
            string sql,
            object? param = null,
            CancellationToken cancellationToken = default);
    }
}
