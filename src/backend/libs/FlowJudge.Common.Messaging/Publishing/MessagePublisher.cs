using Azure.Messaging.ServiceBus;
using FlowJudge.Common.Messaging.Abstractions;
using FlowJudge.Common.Messaging.Outbox;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;

namespace FlowJudge.Common.Messaging.Publishing
{
    internal sealed class MessagePublisher : IPublisher, IOutboxMessagePublisher
    {
        private readonly ServiceBusClient _serviceBusClient;
        private readonly JsonSerializerOptions _jsonSerializerOptions;
        private readonly ILogger<MessagePublisher> _logger;
        private readonly ConcurrentDictionary<string, ServiceBusSender> _senders = new();

        public MessagePublisher(
            ServiceBusClient serviceBusClient,
            ILogger<MessagePublisher> logger)
        {
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _jsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false
            };
        }

        public async Task PublishOutboxMessageAsync(
            Guid messageId,
            string messageType,
            byte[] messagePayload,
            string publishSubject,
            CancellationToken cancellationToken)
        {
            var body = BinaryData.FromBytes(messagePayload);

            await PublishAsync(messageId, messageType, body, publishSubject, cancellationToken);
        }

        public async Task PublishAsync(
            IMessage message,
            string publishSubject,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(publishSubject))
                throw new PublishMessageException(message.MessageId, message.GetType().Name, string.Empty,
                    "Publish subject cannot be null or whitespace.");

            var body = BinaryData.FromBytes(SerializationHelper.Serialize(message));

            await PublishAsync(message.MessageId, SerializationHelper.GetType(message), body, publishSubject, cancellationToken);
        }

        private async Task PublishAsync(Guid messageId,
            string messageType,
            BinaryData messagePayload,
            string publishSubject,
            CancellationToken cancellationToken)
        {
            var sender = _senders.GetOrAdd(
                publishSubject,
                subject => _serviceBusClient.CreateSender(subject));

            var messageIdString = messageId.ToString("N");

            var messageToSend = new ServiceBusMessage(messagePayload)
            {
                MessageId = messageIdString,
                ContentType = "application/json",
                Subject = messageType
            };

            messageToSend.ApplicationProperties[MessageApplicationProperties.EventIdPropertyName] = messageIdString;
            messageToSend.ApplicationProperties[MessageApplicationProperties.EventTypePropertyName] = messageType;

            try
            {
                await sender.SendMessageAsync(messageToSend, cancellationToken);

                _logger.LogInformation("Message of type '{Type}' with Id '{Id}' successfully published on topic/queue '{Subject}'",
                    messageType, messageIdString, publishSubject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new PublishMessageException(messageId, messageType, publishSubject, ex.Message);
            }
        }

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
