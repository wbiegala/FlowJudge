using Microsoft.Extensions.DependencyInjection;
using FlowJudge.Common.Sql.Connection;
using FlowJudge.Common.Sql.Connection.Impl;
using System;
using System.Collections.Generic;
using System.Text;

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

            services.AddTransient<IDbConnectionProvider>(sp =>
            {
                var cfg = sp.GetRequiredService<DatabaseConfiguration>();

                return new PostgresDbConnectionProvider(cfg.ConnectionString);
            });

            return services;
        }
    }
}
