using FlowJudge.GitHub.Webhooks;
using FlowJudge.GitHub.Webhooks.Contract.Events;
using FlowJudge.VCS.Contracts.Events;
using FlowJudge.VCS.Contracts.Events.Data;
using FlowJudge.VCS.Worker.EventPublishing;
using FlowJudge.VCS.Worker.GitHub.WebhooksServices;
using System.Text.Json;

namespace FlowJudge.VCS.Worker.GitHub.WebhookHandlers.Impl
{
    internal sealed class InstallationRepositoriesWebhookHandler : WebhookHandlerBase, IGitHubWebhookHandler
    {
        public InstallationRepositoriesWebhookHandler(IEventPublisher eventPublisher) : base(eventPublisher) { }

        public bool CanHandle(GitHubEventType type) => type == GitHubEventType.InstallationRepositories;

        public async Task HandleAsync(GitHubWebhookMetadata webhook, CancellationToken ct = default)
        {
            var webhookBody = JsonSerializer.Deserialize<InstallationRepositoriesEvent>(webhook.SerializedContent);
            if (webhookBody is null)
                throw new InvalidOperationException($"Invalid event content. Expected {nameof(InstallationRepositoriesEvent)}");

            var publishTask = webhookBody.RepositorySelection == "all"
                ? SendEventForWholeInstallationAsync(webhook, webhookBody, ct)
                : SendEventsForEveryRepositoryAsync(webhook, webhookBody, ct);

            await publishTask;
        }

        private async Task SendEventForWholeInstallationAsync(
            GitHubWebhookMetadata webhook,
            InstallationRepositoriesEvent webhookBody,
            CancellationToken ct = default)
        {
            var @event = new Event<IntegrationChangedEventData>()
            {
                EventId = Guid.NewGuid(),
                Provider = Contracts.VersionControlProvider.GitHub,
                Type = EventType.IntegrationEvent,
                ExternalEventId = webhook.WebhookId.ToString(),
                Data = new IntegrationChangedEventData
                {
                    IntegrationId = webhookBody.Installation.Id.ToString(),
                    Action = IntegrationChangedEventData.IntegrationAction.PermissionsChanged
                }
            };

            await _eventPublisher.PublishAsync(@event, ct);
        }

        private async Task SendEventsForEveryRepositoryAsync(
            GitHubWebhookMetadata webhook,
            InstallationRepositoriesEvent webhookBody,
            CancellationToken ct = default)
        {
            var @events = MapToRepositoryEvents(webhook, webhookBody);

            var publicationTasks = events.Select(e => _eventPublisher.PublishAsync(e, ct));
            await Task.WhenAll(publicationTasks);
        }

        private static IEnumerable<Event<RepositoryChangedEventData>> MapToRepositoryEvents(
            GitHubWebhookMetadata webhook,
            InstallationRepositoriesEvent webhookBody)
        {
            var result = new List<Event<RepositoryChangedEventData>>();

            result.AddRange(webhookBody.RepositoriesAdded.Select(ra => new Event<RepositoryChangedEventData>
            {
                EventId = Guid.NewGuid(),
                Provider = Contracts.VersionControlProvider.GitHub,
                Type = EventType.RepositoryEvent,
                ExternalEventId = webhook.WebhookId.ToString(),
                Data = new RepositoryChangedEventData
                {
                    IntegrationId = webhookBody.Installation.Id.ToString(),
                    Action = RepositoryChangedEventData.RepositoryAction.AccessGranted,
                    RepositoryId = ra.Id.ToString(),
                    RepositoryName = ra.Name,
                    RepositoryFullName = ra.FullName,
                }
            }));

            result.AddRange(webhookBody.RepositoriesRemoved.Select(ra => new Event<RepositoryChangedEventData>
            {
                EventId = Guid.NewGuid(),
                Provider = Contracts.VersionControlProvider.GitHub,
                Type = EventType.RepositoryEvent,
                ExternalEventId = webhook.WebhookId.ToString(),
                Data = new RepositoryChangedEventData
                {
                    IntegrationId = webhookBody.Installation.Id.ToString(),
                    Action = RepositoryChangedEventData.RepositoryAction.AccessRevoked,
                    RepositoryId = ra.Id.ToString(),
                    RepositoryName = ra.Name,
                    RepositoryFullName = ra.FullName,
                }
            }));

            return result;
        }
    }
}
