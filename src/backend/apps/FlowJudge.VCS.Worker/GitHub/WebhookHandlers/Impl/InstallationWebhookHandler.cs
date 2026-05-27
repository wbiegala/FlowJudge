using FlowJudge.GitHub.Webhooks;
using FlowJudge.GitHub.Webhooks.Contract.Events;
using FlowJudge.VCS.Contracts.Events;
using FlowJudge.VCS.Contracts.Events.Data;
using FlowJudge.VCS.Worker.EventPublishing;
using FlowJudge.VCS.Worker.GitHub.WebhooksServices;
using System.Text.Json;
using static FlowJudge.VCS.Contracts.Events.Data.IntegrationChangedEventData;

namespace FlowJudge.VCS.Worker.GitHub.WebhookHandlers.Impl
{
    internal sealed class InstallationWebhookHandler : WebhookHandlerBase, IGitHubWebhookHandler
    {
        public InstallationWebhookHandler(IEventPublisher publisher): base(publisher) { }

        public bool CanHandle(GitHubEventType type) => type == GitHubEventType.Installation;

        public async Task HandleAsync(GitHubWebhookMetadata webhook, CancellationToken ct = default)
        {
            var webhookBody = JsonSerializer.Deserialize<InstallationEvent>(webhook.SerializedContent);
            if (webhookBody is null)
                throw new InvalidOperationException($"Invalid event content. Expected {nameof(InstallationEvent)}");

            var @event = MapToEvent(webhook, webhookBody);

            await _eventPublisher.PublishAsync(@event, ct);
        }

        private static Event<IntegrationChangedEventData> MapToEvent(GitHubWebhookMetadata webhook, InstallationEvent webhookBody)
        {
            return new Event<IntegrationChangedEventData>()
            {
                EventId = Guid.NewGuid(),
                Provider = Contracts.VersionControlProvider.GitHub,
                Type = EventType.IntegrationEvent,
                ExternalEventId = webhook.WebhookId.ToString(),
                Data = new IntegrationChangedEventData
                {
                    IntegrationId = webhookBody.Installation.Id.ToString(),
                    Action = webhookBody.Action switch
                    {
                        "created" => IntegrationAction.Created,
                        "deleted" => IntegrationAction.Deleted,
                        "new_permissions_accepted" => IntegrationAction.PermissionsChanged,
                        "suspend" => IntegrationAction.Deactivated,
                        "unsuspend" => IntegrationAction.Reactivated,
                        _ => throw new InvalidOperationException("Unknow installation action")
                    }
                }
            };
        }
    }
}
