using FlowJudge.API.Service.Auth.Legal;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace FlowJudge.API.Service.Auth
{
    public static class AuthorizationPolicyInstaller
    {
        public static IServiceCollection AddFlowJudgeAuthorization(
            this IServiceCollection services,
            Func<JwtConfiguration> configure)
        {
            ArgumentNullException.ThrowIfNull(configure);

            var configuration = configure() ?? throw new ArgumentNullException(nameof(configure));

            var rsaPublicKey = RSA.Create();
            rsaPublicKey.ImportSubjectPublicKeyInfo(
                Convert.FromBase64String(configuration.PublicKey),
                out _);

            services.AddHttpContextAccessor();

            services.AddScoped<ILegalLockManager, CachedLegalLockManager>();
            services.AddScoped<IAuthorizationHandler, LegalAcceptedAuthorizationHandler>();
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, FlowJudgeAuthorizationMiddlewareResultHandler>();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.MetadataAddress = configuration.MetadataAddress;
                    options.RequireHttpsMetadata = configuration.RequireHttpsMetadata;
                    options.Audience = configuration.Audience;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configuration.Issuer,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.FromSeconds(30),
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new RsaSecurityKey(rsaPublicKey)
                    };
                });

            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser()
                    .AddRequirements(new LegalAcceptedRequirement())
                    .Build();
            });


            return services;
        }
    }
}
