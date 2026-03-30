using Microsoft.Extensions.DependencyInjection;
using FlowJudge.Common.Utils.Time;

namespace FlowJudge.Common.Utils
{
    public static class Installer
    {
        public static IServiceCollection AddTimeService(this IServiceCollection services)
        {
            services.AddSingleton<ITimeService, TimeService>();

            return services;
        }
    }
}
