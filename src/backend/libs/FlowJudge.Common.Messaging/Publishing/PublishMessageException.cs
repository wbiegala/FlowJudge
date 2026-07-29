using System.Text;

namespace FlowJudge.Common.Messaging.Publishing
{
    public sealed class PublishMessageException : Exception
    {
        public Guid MessageId { get; }
        public string MessageType { get; }
        public string PublishSubject { get; }
        public string? Reason { get; }

        public PublishMessageException(Guid messageId, string messageType, string publishSubject, string? reason = null)
            : base(ExceptionMessage(messageId, messageType, publishSubject, reason))
        {
            MessageId = messageId;
            MessageType = messageType;
            PublishSubject = publishSubject;
            Reason = reason;
        }

        private static string ExceptionMessage(Guid messageId, string messageType, string publishSubject, string? reason = null)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Error occurred while publishing message of type '{messageType}' with Id '{messageId}' on topic/queue '{publishSubject}'.");

            if (!string.IsNullOrWhiteSpace(reason))
            {
                builder.AppendLine($"Reason: {reason}");
            }

            return builder.ToString();
        }
    }
}
