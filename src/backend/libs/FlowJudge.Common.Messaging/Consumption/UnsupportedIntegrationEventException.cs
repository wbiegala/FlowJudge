namespace FlowJudge.Common.Messaging.Consumption
{
    internal sealed class UnsupportedIntegrationEventException : Exception
    {
        public UnsupportedIntegrationEventException(string message)
            : base(message)
        {
        }

        public UnsupportedIntegrationEventException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
