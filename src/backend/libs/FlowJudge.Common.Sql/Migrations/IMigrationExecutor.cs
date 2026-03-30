namespace FlowJudge.Common.Sql.Migrations
{
    public interface IMigrationExecutor
    {
        Task ExecuteAsync(CancellationToken ct = default);
    }
}
