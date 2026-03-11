using FlowJudge.API.Service.Auth;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddKeycloakAuthentication(options =>
{
    options.UseKeycloak(
        baseUrl: builder.Configuration["Auth:Keycloak:BaseUrl"] ?? string.Empty,
        baseUrlInternal: builder.Configuration["Auth:Keycloak:BaseUrlInternal"] ?? string.Empty,
        realm: builder.Configuration["Auth:Keycloak:Realm"] ?? string.Empty);
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
