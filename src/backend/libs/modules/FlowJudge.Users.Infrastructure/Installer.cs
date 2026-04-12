using FlowJudge.Users.Infrastructure.Repositories.DocumentVersions;
using FlowJudge.Users.Infrastructure.Repositories.Users;
using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.Users.Infrastructure
{
    public static class Installer
    {
        public static IServiceCollection AddUsersInfrastructure(this IServiceCollection services)
        {
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IDocumentVersionRepository, DocumentVersionRepository>();


            return services;
        }
    }
}
