using System.Text.Json;

namespace FlowJudge.Common.Messaging.Outbox
{
    internal static class SerializationHelper
    {
        public static string GetType(IMessage message)
        {
            return message.GetType().AssemblyQualifiedName ?? throw new InvalidOperationException("Failed to get the assembly qualified name of the message type.");
        }

        public static byte[] Serialize<T>(T obj)
        {
            if (obj is null)
                throw new ArgumentNullException(nameof(obj));

            var messageConcreteType = obj.GetType();
            return JsonSerializer.SerializeToUtf8Bytes(obj, messageConcreteType, Options);
        }

        public static T? Deserialize<T>(byte[] data)
        {
            return JsonSerializer.Deserialize<T>(data, Options);
        }

        private static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
    }
}
