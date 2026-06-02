using FlowJudge.Common.Messaging.Abstractions;

namespace FlowJudge.VCS.Worker.GitHub.WebhookHandlers.Impl
{
    internal abstract class WebhookHandlerBase
    {
        protected readonly IPublisher _publisher;

        protected WebhookHandlerBase(IPublisher publisher)
        {
            _publisher = publisher;
        }
    }
}
