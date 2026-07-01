using FlowJudge.Common.Messaging.Abstractions;
using FlowJudge.GitHub.Webhooks;
using FlowJudge.GitHub.Webhooks.Contract.Events;
using FlowJudge.VCS.Contracts.Events;
using FlowJudge.VCS.Contracts.Shared;
using FlowJudge.VCS.Worker.GitHub.WebhooksServices;
using System.Text.Json;
using static FlowJudge.VCS.Contracts.Events.IntegrationChangedEvent;

namespace FlowJudge.VCS.Worker.GitHub.WebhookHandlers.Impl
{
    internal sealed class InstallationRepositoriesWebhookHandler : WebhookHandlerBase, IGitHubWebhookHandler
    {
        public InstallationRepositoriesWebhookHandler(IPublisher publisher) : base(publisher)
        {
        }

        public bool CanHandle(GitHubEventType type) =>
            type == GitHubEventType.InstallationRepositories;

        public async Task HandleAsync(GitHubWebhookMetadata webhook, CancellationToken ct = default)
        {
            var webhookMessage = JsonSerializer.Deserialize<InstallationRepositoriesEvent>(webhook.SerializedContent);
            if (webhookMessage is null)
                throw new InvalidOperationException($"Failed to deserialize webhook content to {nameof(InstallationRepositoriesEvent)}.");

            var message = new IntegrationChangedEvent()
            {
                MessageId = Guid.NewGuid(),
                EventType = EventType.IntegrationEvent,
                Provider = IntegrationProvider.GitHub,
                Integration = new IntegrationChangedEvent.IntegrationData
                {
                    IntegrationId = webhookMessage.Installation.Id.ToString(),
                },
                Action = IntegrationAction.Updated
            };

            await _publisher.PublishAsync(message, Topics.IntegrationEventTopic, ct);
        }
    }
}
