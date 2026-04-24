using FlowJudge.Workspaces.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.Workspaces.Application
{
    public static class Installer
    {
        public static IServiceCollection AddWorkspacesModule(this IServiceCollection services)
        {
            services.AddWorkspacesInfrastructure();

            return services;
        }
    }
}
