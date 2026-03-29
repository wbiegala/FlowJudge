using FlowJudge.API.Service.Auth;
using FlowJudge.API.Service.ErrorHandling;
using FlowJudge.Common.Cache;
using FlowJudge.Common.Utils;

var builder = WebApplication.CreateBuilder(args);

var dbConnectionString = builder.Configuration.GetConnectionString("Postgres");
var redisConnectionString = builder.Configuration.GetConnectionString("Redis");

builder.Services.AddCors(options =>
{
    options.AddPolicy("WebApp", policy =>
    {
        var configValue = builder.Configuration["CorsAllowedOrigins"] ?? string.Empty;
        var corsAllowedOrigins = configValue
            .Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        policy.WithOrigins(corsAllowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddKeycloakAuthentication(options =>
{
    options.UseKeycloak(
        baseUrl: builder.Configuration["Auth:Keycloak:BaseUrl"] ?? string.Empty,
        baseUrlInternal: builder.Configuration["Auth:Keycloak:BaseUrlInternal"] ?? string.Empty,
        realm: builder.Configuration["Auth:Keycloak:Realm"] ?? string.Empty,
        clientId: builder.Configuration["Auth:Keycloak:ClientId"] ?? string.Empty,
        clientSecret: builder.Configuration["Auth:Keycloak:ClientSecret"] ?? string.Empty,
        registrationCallbackUri: builder.Configuration["Auth:Keycloak:RegistrationCallbackUri"] ?? string.Empty,
        loginCallbackUri: builder.Configuration["Auth:Keycloak:LoginCallbackUri"] ?? string.Empty);      
});
builder.Services.AddJwtAuthorization(() => builder.Configuration.GetSection("Auth:JWT").Get<JwtConfiguration>()!);

builder.Services.AddCache(cfg =>
{
    cfg.UseRedis(redisConnectionString!);
    cfg.UsePostgreSql(dbConnectionString!);
});

builder.Services.AddTimeService();

builder.Services.AddControllers();

builder.Services.AddOpenApi();

builder.Services.AddScoped<ErrorHandlerMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCors("WebApp");

app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
