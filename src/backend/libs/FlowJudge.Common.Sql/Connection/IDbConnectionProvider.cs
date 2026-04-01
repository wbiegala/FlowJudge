using System.Data.Common;

namespace FlowJudge.Common.Sql.Connection
{
    public interface IDbConnectionProvider
    {
        DbConnection GetDbConnection();
    }
}
