using FlowJudge.Common.Application;
using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.Workspaces.Application
{
    public static class Installer
    {
        public static IServiceCollection AddWorkspacesApplication(this IServiceCollection services)
        {
            services.AddMediator(typeof(Installer).Assembly);

            return services;
        }
    }
}
