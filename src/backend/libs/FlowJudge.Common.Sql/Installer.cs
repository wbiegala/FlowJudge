using FlowJudge.Common.Sql.Connection;
using FlowJudge.Common.Sql.Connection.Impl;
using FlowJudge.Common.Sql.Migrations;
using FlowJudge.Common.Sql.Migrations.Executors;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Common.Sql.UnitOfWork.Impl;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Reflection;

namespace FlowJudge.Common.Sql
{
    public static class Installer
    {
        public static IServiceCollection AddPostgresDatabase(
            this IServiceCollection services,
            Action<DatabaseConfiguration> configure)
        {
            var configuration = new DatabaseConfiguration();
            configure(configuration);
            services.AddSingleton(configuration);

            if (configuration.InstallMigrations)
            {
                services.AddPostgresMigrations(configuration.MigrationAssemblies.ToArray());
            }

            services.AddSingleton<IDbConnectionProvider>(sp =>
            {
                var cfg = sp.GetRequiredService<DatabaseConfiguration>();

                return new PostgresDbConnectionProvider(cfg.ConnectionString);
            });

            services.AddScoped<ISqlSession, SqlSession>();
            services.AddScoped<IUnitOfWork, UnitOfWorkImpl>();

            return services;
        }

        private static IServiceCollection AddPostgresMigrations(this IServiceCollection services, Assembly[] assemblies)
        {
            services.AddSingleton<IMigrationExecutor, PostgresMigrationExecutor>();

            foreach (var assembly in assemblies)
            {
                var migrationTypes = assembly
                    .GetTypes()
                    .Where(type =>
                        typeof(IMigration).IsAssignableFrom(type) &&
                        type is { IsClass: true, IsAbstract: false });

                foreach (var migrationType in migrationTypes)
                {
                    services.TryAddEnumerable(
                        ServiceDescriptor.Singleton(typeof(IMigration), migrationType));
                }
            }

            return services;
        }
    }
}
