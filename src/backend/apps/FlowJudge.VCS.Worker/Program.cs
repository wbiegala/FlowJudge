using Azure.Messaging.ServiceBus;
using FlowJudge.GitHub.Webhooks;
using FlowJudge.VCS.Worker.EventPublishing;
using FlowJudge.VCS.Worker.GitHub.WebhookHandlers;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var webhookSecret = builder.Configuration.GetValue<string>("GitHub:WebhooksSecret")
    ?? throw new InvalidOperationException("No webhooks secret provided");

builder.Services.AddSingleton(serviceProvider =>
{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

    var connectionString = configuration["AzureWebJobsServiceBus"];

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "Missing required configuration value: AzureWebJobsServiceBus.");
    }

    return new ServiceBusClient(connectionString);
});

builder.Services.AddSingleton<IEventPublisher, EventPublisher>();

builder.Services.AddGitHubWebhookHandlers();
builder.Services.AddGitHubWebhooks(webhookSecret);
// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
