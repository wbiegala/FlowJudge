using System.Text;

namespace FlowJudge.Common.Messaging.Publishing
{
    public sealed class PublishMessageException : Exception
    {
        private readonly IMessage _message;

        public Guid MessageId => _message.MessageId;
        public Type MessageType => _message.GetType();
        public string PublishSubject { get; }
        public string? Reason { get; }

        public PublishMessageException(IMessage message, string publishSubject, string? reason = null)
            : base(ExceptionMessage(message, publishSubject, reason))
        {
            _message = message;
            PublishSubject = publishSubject;
            Reason = reason;
        }

        private static string ExceptionMessage(IMessage message, string publishSubject, string? reason = null)
        {
            var builder = new StringBuilder();
            builder.AppendLine($"Error occurred while publishing message of type '{message.GetType().Name}' with Id '{message.MessageId}' on topic/queue '{publishSubject}'.");

            if (!string.IsNullOrWhiteSpace(reason))
            {
                builder.AppendLine($"Reason: {reason}");
            }

            return builder.ToString();
        }
    }
}
