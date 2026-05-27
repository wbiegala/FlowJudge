using Azure.Messaging.ServiceBus;
using FlowJudge.VCS.Contracts.Events;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;

namespace FlowJudge.VCS.Worker.EventPublishing
{
    internal sealed class EventPublisher : IEventPublisher, IAsyncDisposable
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<EventPublisher> _logger;
        private readonly ConcurrentDictionary<string, ServiceBusSender> _senders = new();

        public EventPublisher(
            ServiceBusClient serviceBusClient,
            ILogger<EventPublisher> logger)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;

            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task PublishAsync<TEventData>(
            Event<TEventData> @event,
            CancellationToken cancellationToken = default)
            where TEventData : class, IEventData
        {
            var topic = GetTopicByEventType(@event.Type);

            if (string.IsNullOrWhiteSpace(topic))
            {
                _logger.LogWarning(
                    "EventId={EventId} - no topic for event type of {Type}",
                    @event.EventId,
                    @event.Type);

                return;
            }

            var sender = _senders.GetOrAdd(
                topic,
                topicName => _serviceBusClient.CreateSender(topicName));

            var body = BinaryData.FromObjectAsJson(@event, _jsonSerializerOptions);

            var message = new ServiceBusMessage(body)
            {
                MessageId = @event.EventId.ToString("N"),
                ContentType = "application/json",
                Subject = @event.Type.ToString()
            };

            message.ApplicationProperties["eventId"] = @event.EventId.ToString("N");
            message.ApplicationProperties["eventType"] = @event.Type.ToString();
            message.ApplicationProperties["eventDataType"] = typeof(TEventData).Name;
            message.ApplicationProperties["eventDataFullType"] = typeof(TEventData).FullName ?? typeof(TEventData).Name;

            await sender.SendMessageAsync(message, cancellationToken);

            _logger.LogInformation(
                "EventId={EventId} - event published on topic {Topic}",
                @event.EventId,
                topic);
        }

        private static string GetTopicByEventType(EventType type) =>
            type switch
            {
                EventType.IntegrationEvent => Topics.IntegrationEventTopic,
                EventType.RepositoryEvent => Topics.RepositoryEventTopic,
                EventType.PullRequestEvent => Topics.PullRequestEventTopic,
                EventType.CodeReviewEvent => Topics.CodeReviewEventTopic,
                _ => string.Empty
            };

        public async ValueTask DisposeAsync()
        {
            foreach (var sender in _senders.Values)
            {
                await sender.DisposeAsync();
            }

            _senders.Clear();
        }
    }
}
