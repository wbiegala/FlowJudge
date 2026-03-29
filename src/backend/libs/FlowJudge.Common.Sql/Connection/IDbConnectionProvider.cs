using System.Data;

namespace FlowJudge.Common.Sql.Connection
{
    public interface IDbConnectionProvider
    {
        IDbConnection GetDbConnection();
    }
}
