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
    internal sealed class InstallationWebhookHandler : WebhookHandlerBase, IGitHubWebhookHandler
    {
        public InstallationWebhookHandler(IPublisher publisher)
            : base(publisher)
        {
        }

        public bool CanHandle(GitHubEventType type) =>
            type == GitHubEventType.Installation;

        public async Task HandleAsync(GitHubWebhookMetadata webhook, CancellationToken ct = default)
        {
            var webhookMessage = JsonSerializer.Deserialize<InstallationEvent>(webhook.SerializedContent);
            if (webhookMessage is null)
                throw new InvalidOperationException($"Failed to deserialize webhook content to {nameof(InstallationEvent)}.");

            var eventAction = webhookMessage.Action switch
            {
                "created" => IntegrationAction.Created,
                "deleted" => IntegrationAction.Deleted,
                "new_permissions_accepted" => IntegrationAction.Updated,
                "suspend" => IntegrationAction.Deactivated,
                "unsuspend" => IntegrationAction.Reactivated,
                _ => throw new InvalidOperationException("Unknown action.")
            };

            var message = new IntegrationChangedEvent()
            {
                MessageId = Guid.NewGuid(),
                EventType = EventType.IntegrationEvent,
                Provider = IntegrationProvider.GitHub,
                Integration = new IntegrationChangedEvent.IntegrationData
                { 
                    IntegrationId = webhookMessage.Installation.Id.ToString(),
                },
                Action = eventAction
            };

            await _publisher.PublishAsync(message, Topics.IntegrationEventTopic, ct);
        }
    }
}
