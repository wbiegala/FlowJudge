using FlowJudge.GitHub.Webhooks;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

var webhookSecret = builder.Configuration.GetValue<string>("GitHub:WebhooksSecret")
    ?? throw new InvalidOperationException("No webhooks secret provided");

builder.Services.AddGitHubWebhooks(webhookSecret);
// Application Insights isn't enabled by default. See https://aka.ms/AAt8mw4.
// builder.Services
//     .AddApplicationInsightsTelemetryWorkerService()
//     .ConfigureFunctionsApplicationInsights();

builder.Build().Run();
