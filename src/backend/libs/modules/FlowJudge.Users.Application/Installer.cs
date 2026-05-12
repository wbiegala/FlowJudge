using FlowJudge.Common.Application;
using FlowJudge.Users.Application.Services.Domain;
using FlowJudge.Users.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FlowJudge.Users.Application
{
    public static class Installer
    {
        public static IServiceCollection AddUsersApplication(this IServiceCollection services)
        {
            services.AddMediator(typeof(Installer).Assembly);

            services.AddScoped<IUserLegalStateService, UserLegalStateService>();

            return services;
        }
    }
}
