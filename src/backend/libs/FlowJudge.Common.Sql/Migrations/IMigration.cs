namespace FlowJudge.Common.Sql.Migrations
{
    public interface IMigration
    {
        Task ExecuteAsync();
    }
}
