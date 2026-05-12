using FlowJudge.Workspaces.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Infrastructure.Repositories.Workspaces;
using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.Workspaces.Infrastructure
{
    public static class Installer
    {
        public static IServiceCollection AddWorkspacesInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();


            return services;
        }
    }
}
