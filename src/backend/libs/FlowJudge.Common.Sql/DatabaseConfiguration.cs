namespace FlowJudge.Common.Sql
{
    public sealed class DatabaseConfiguration
    {
        internal string ConnectionString { get; private set; } = string.Empty;

        public void WithConnectionString(string connectionString)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }
    }
}
