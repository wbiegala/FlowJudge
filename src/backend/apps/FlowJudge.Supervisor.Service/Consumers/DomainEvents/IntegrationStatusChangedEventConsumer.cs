using FlowJudge.Common.Messaging;
using FlowJudge.Reviews.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Events.Integrations;

namespace FlowJudge.Supervisor.Service.Consumers.DomainEvents
{
    public class IntegrationStatusChangedEventConsumer : IConsumer<IntegrationStatusChangedEvent>
    {
        private readonly IDomainSynchronizer _synchronizer;

        public IntegrationStatusChangedEventConsumer(IDomainSynchronizer synchronizer)
        {
            _synchronizer = synchronizer;
        }

        public async Task ConsumeAsync(IntegrationStatusChangedEvent message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
