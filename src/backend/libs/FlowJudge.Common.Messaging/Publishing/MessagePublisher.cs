using Azure.Messaging.ServiceBus;
using FlowJudge.Common.Messaging.Abstractions;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Text.Json;

namespace FlowJudge.Common.Messaging.Publishing
{
    internal sealed class MessagePublisher : IPublisher
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

        public async Task PublishAsync(
            IMessage message,
            string publishSubject,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(publishSubject))
                throw new PublishMessageException(message, string.Empty,
                    "Publish subject cannot be null or whitespace.");            

            var sender = _senders.GetOrAdd(
                publishSubject,
                subject => _serviceBusClient.CreateSender(subject));

            var body = BinaryData.FromObjectAsJson(message, _jsonSerializerOptions);

            var messageTypeName = message.GetType().Name;
            var messageIdString = message.MessageId.ToString("N");

            var messageToSend = new ServiceBusMessage(body)
            {
                MessageId = messageIdString,
                ContentType = "application/json",
                Subject = messageTypeName
            };

            messageToSend.ApplicationProperties[MessageApplicationProperties.EventIdPropertyName] = messageIdString;
            messageToSend.ApplicationProperties[MessageApplicationProperties.EventTypePropertyName] = messageTypeName;

            try
            {
                await sender.SendMessageAsync(messageToSend, cancellationToken);

                _logger.LogInformation("Message of type '{Type}' with Id '{Id}' successfully published on topic/queue '{Subject}'",
                    messageTypeName, messageIdString, publishSubject);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw new PublishMessageException(message, publishSubject, ex.Message);
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
