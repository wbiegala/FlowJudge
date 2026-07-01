namespace FlowJudge.Common.Messaging.Abstractions
{
    public interface IPublisher : IAsyncDisposable
    {
        /// <summary>
        /// Publishes a message to the specified topic or queue.
        /// </summary>
        /// <param name="message">Message to publish</param>
        /// <param name="publishSubject">Topic or queue name</param>
        Task PublishAsync(
            IMessage message,
            string publishSubject,
            CancellationToken cancellationToken = default);
    }
}
