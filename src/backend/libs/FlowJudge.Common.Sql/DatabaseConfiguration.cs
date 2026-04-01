using System.Reflection;

namespace FlowJudge.Common.Sql
{
    public sealed class DatabaseConfiguration
    {
        private readonly List<Assembly> _migrationAssemblies = new List<Assembly>();

        internal bool InstallMigrations { get; private set; } = false;
        internal IEnumerable<Assembly> MigrationAssemblies => _migrationAssemblies.Distinct();
        internal string ConnectionString { get; private set; } = string.Empty;

        public void WithConnectionString(string connectionString)
        {
            ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        public void WithDatabaseMigrationsFromAssembly(Assembly assembly)
        {
            InstallMigrations = true;
            _migrationAssemblies.Add(assembly);
        }
    }
}
