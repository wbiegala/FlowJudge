using FlowJudge.Common.Application;
using FlowJudge.Workspaces.Application.Abstractions.Services;
using FlowJudge.Workspaces.Application.Services.GitHubInstallation;
using FlowJudge.Workspaces.Domain;
using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.Workspaces.Application
{
    public static class Installer
    {
        public static IServiceCollection AddWorkspacesApplication(this IServiceCollection services)
        {
            services.AddWorkspacesDomainServices();
            services.AddMediator(typeof(Installer).Assembly);
            services.AddScoped<IGitHubInstallationService, GitHubInstallationService>();

            return services;
        }
    }
}
