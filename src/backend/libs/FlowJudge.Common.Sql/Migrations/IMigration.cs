using System.Data;

namespace FlowJudge.Common.Sql.Migrations
{
    public interface IMigration
    {
        Task ExecuteAsync(IMigrationContext migrationContext);
    }
}
