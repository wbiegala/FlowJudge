using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System.Text.Json;

namespace FlowJudge.Common.Messaging.Consumption
{
    internal sealed class MessageDispatcher
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly JsonSerializerOptions _jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            WriteIndented = false
        };

        public MessageDispatcher(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task DispatchAsync(
            ServiceBusReceivedMessage message,
            ConsumerOptions options,
            CancellationToken ct)
        {
            object? deserializedMessage;

            try
            {
                deserializedMessage = JsonSerializer.Deserialize(
                    message.Body.ToString(),
                    options.MessageType,
                    _jsonSerializerOptions);
            }
            catch (Exception exception) when (exception is JsonException or NotSupportedException)
            {
                throw new UnsupportedIntegrationEventException(
                    $"Message from '{options.ConsumerKey}' cannot be deserialized as '{options.MessageType.FullName}'.",
                    exception);
            }

            if (deserializedMessage is not IMessage typedMessage)
            {
                throw new UnsupportedIntegrationEventException(
                    $"Message from '{options.ConsumerKey}' cannot be deserialized as '{options.MessageType.FullName}'.");
            }

            await using var scope = _scopeFactory.CreateAsyncScope();
            var consumerFactory = scope.ServiceProvider.GetRequiredService<IConsumerFactory>();
            var consumer = consumerFactory.GetConsumer(options);

            var consumeMethod = GetConsumeMethod(options.ConsumerType, options.MessageType);
            var result = consumeMethod.Invoke(
                consumer,
                new object[] { typedMessage, ct }) as Task;

            if (result is null)
            {
                throw new InvalidOperationException(
                    $"Consumer '{options.ConsumerType.FullName}' did not return a task from ConsumeAsync.");
            }

            await result;
        }

        private static MethodInfo GetConsumeMethod(Type consumerType, Type messageType)
        {
            var consumerInterface = typeof(IConsumer<>).MakeGenericType(messageType);

            if (!consumerInterface.IsAssignableFrom(consumerType))
            {
                throw new InvalidOperationException(
                    $"Consumer '{consumerType.FullName}' is not registered for message '{messageType.FullName}'.");
            }

            return consumerInterface.GetMethod(nameof(IConsumer<IMessage>.ConsumeAsync))
                ?? throw new InvalidOperationException(
                    $"Consumer '{consumerType.FullName}' does not expose ConsumeAsync.");
        }
    }
}
