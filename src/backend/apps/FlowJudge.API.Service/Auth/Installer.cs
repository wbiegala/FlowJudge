using FlowJudge.API.Service.Auth.Keycloak;
using FlowJudge.API.Service.Auth.Keycloak.Client;

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

            services.AddHttpClient<IKeycloakClient, KeycloakHttpClient>((ctx, client) =>
            {
                var configuration = ctx.GetRequiredService<KeycloakAuthenticationConfiguration>();
                client.Timeout = TimeSpan.FromSeconds(30);
                client.BaseAddress = new Uri(configuration.BaseUrlInternal);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });

            return services;
        }
    }
}
