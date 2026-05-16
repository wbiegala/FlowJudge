using FlowJudge.Workspaces.Domain.Integration.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.Workspaces.Domain
{
    public static class DomainServicesInstaller
    {
        public static IServiceCollection AddWorkspacesDomainServices(this IServiceCollection services)
        {
            services.AddScoped<IIntegrationFactory, Integration.Services.Impl.IntegrationFactory>();

            return services;
        }
    }
}
