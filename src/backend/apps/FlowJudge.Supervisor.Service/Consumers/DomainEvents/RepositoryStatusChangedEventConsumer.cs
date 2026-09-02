using FlowJudge.Common.Messaging;
using FlowJudge.Reviews.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Events.Repositories;

namespace FlowJudge.Supervisor.Service.Consumers.DomainEvents
{
    public class RepositoryStatusChangedEventConsumer : IConsumer<RepositoryStatusChangedEvent>
    {
        private readonly IDomainSynchronizer _synchronizer;

        public RepositoryStatusChangedEventConsumer(IDomainSynchronizer synchronizer)
        {
            _synchronizer = synchronizer;
        }

        public async Task ConsumeAsync(RepositoryStatusChangedEvent message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
