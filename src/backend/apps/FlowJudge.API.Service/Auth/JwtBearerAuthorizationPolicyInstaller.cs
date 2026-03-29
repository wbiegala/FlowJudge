using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace FlowJudge.API.Service.Auth
{
    public static class JwtBearerAuthorizationPolicyInstaller
    {
        public static IServiceCollection AddJwtAuthorization(this IServiceCollection services, Func<JwtConfiguration> configure)
        {
            var configuration = configure() ?? throw new ArgumentNullException(nameof(configure));

            var rsaPublicKey = RSA.Create();
            rsaPublicKey.ImportSubjectPublicKeyInfo(Convert.FromBase64String(configuration.PublicKey), out _);

            services.AddAuthorization();
            services.AddAuthentication("Bearer")
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

            return services;
        }
    }
}
