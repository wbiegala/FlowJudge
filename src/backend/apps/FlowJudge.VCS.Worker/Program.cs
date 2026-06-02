using FlowJudge.Common.Messaging;
using FlowJudge.GitHub.Webhooks;
using FlowJudge.VCS.Worker.GitHub.WebhookHandlers;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var webhookSecret = builder.Configuration.GetValue<string>("GitHub:WebhooksSecret")
    ?? throw new InvalidOperationException("No webhooks secret provided");

var serviceBusConnectionString = builder.Configuration.GetValue<string>("AzureWebJobsServiceBus");
if (string.IsNullOrWhiteSpace(serviceBusConnectionString))
{
    throw new InvalidOperationException("No Service Bus connection string provided");
}

builder.Services.AddAzureServiceBus(cfg =>
{
    cfg.WithConnectionString(serviceBusConnectionString);
});


builder.Services.AddGitHubWebhookHandlers();
builder.Services.AddGitHubWebhooks(webhookSecret);
// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
