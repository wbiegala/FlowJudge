using FlowJudge.Common.Messaging;
using FlowJudge.Reviews.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Events.Integrations;

namespace FlowJudge.Supervisor.Service.Consumers.DomainEvents
{
    public class IntegrationCreatedEventConsumer : IConsumer<IntegrationCreatedEvent>
    {
        private readonly IDomainSynchronizer _synchronizer;

        public IntegrationCreatedEventConsumer(IDomainSynchronizer synchronizer)
        {
            _synchronizer = synchronizer;
        }

        public async Task ConsumeAsync(IntegrationCreatedEvent message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
