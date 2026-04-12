using FlowJudge.Common.Application;
using FlowJudge.Users.Application.Services.Domain;
using FlowJudge.Users.Domain.Services;
using FlowJudge.Users.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.Users.Application
{
    public static class Installer
    {
        public static IServiceCollection AddUsersModule(this IServiceCollection services)
        {
            services.AddMediator(typeof(Installer).Assembly);
            services.AddUsersInfrastructure();

            services.AddScoped<IUserLegalStateService, UserLegalStateService>();

            return services;
        }
    }
}
