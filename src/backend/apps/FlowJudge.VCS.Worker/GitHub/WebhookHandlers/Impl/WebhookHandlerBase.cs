using FlowJudge.VCS.Worker.EventPublishing;

namespace FlowJudge.VCS.Worker.GitHub.WebhookHandlers.Impl
{
    internal abstract class WebhookHandlerBase
    {
        protected readonly IEventPublisher _eventPublisher;

        protected WebhookHandlerBase(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }
    }
}
