using System.Reflection;

namespace FlowJudge.Common.Sql.Migrations
{
    public sealed class MigrationConfiguration
    {
        private List<Assembly> _assemblies = new List<Assembly>();
        internal IEnumerable<Assembly> Assemblies => _assemblies;

        public void AddMigrationsFromAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            _assemblies.Add(assembly);
        }
    }
}
