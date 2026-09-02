using FlowJudge.Common.Messaging;
using FlowJudge.Supervisor.Service.Consumers.DomainEvents;
using FlowJudge.Workspaces.Domain.Events.Integrations;
using FlowJudge.Workspaces.Domain.Events.Repositories;
using FlowJudge.Workspaces.Domain.Events.Workspace;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ServiceBus");

if (string.IsNullOrWhiteSpace(connectionString))
    throw new InvalidOperationException("Service Bus connection string is not configured.");

builder.Services.AddAzureServiceBus(cfg =>
{
    cfg.WithConnectionString(connectionString);

    // DOMAIN EVENTS
    cfg.WithConsumers(c =>
    {  
        c.AddConsumerForTopic<IntegrationCreatedEventConsumer, IntegrationCreatedEvent>(SubscriptionName.Name);
        c.AddConsumerForTopic<IntegrationStatusChangedEventConsumer, IntegrationStatusChangedEvent>(SubscriptionName.Name);
        c.AddConsumerForTopic<RepositoryStatusChangedEventConsumer, RepositoryStatusChangedEvent>(SubscriptionName.Name);
        c.AddConsumerForTopic<WorkspaceCreatedEventConsumer, WorkspaceCreatedEvent>(SubscriptionName.Name);
        c.AddConsumerForTopic<WorkspaceStatusChangedEventConsumer, WorkspaceStatusChangedEvent>(SubscriptionName.Name);
    });
});

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
