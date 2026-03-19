using FlowJudge.API.Service.Auth.Keycloak;

namespace FlowJudge.API.Service.Auth
{
    public static class Installer
    {
        public static IServiceCollection AddKeycloakAuthentication(
            this IServiceCollection services,
            Action<AuthenticationConfiguration> configure)
        {
            var configuration = new AuthenticationConfiguration();
            configure(configuration);

            services.AddSingleton(_ => configuration.BuildKeycloakConfiguration());

            services.AddScoped<IAuthenticationService, KeycloakAuthenticationService>();

            return services;
        }
    }
}
