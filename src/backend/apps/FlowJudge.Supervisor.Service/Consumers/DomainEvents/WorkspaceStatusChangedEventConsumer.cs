using FlowJudge.Common.Messaging;
using FlowJudge.Reviews.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Events.Workspace;

namespace FlowJudge.Supervisor.Service.Consumers.DomainEvents
{
    public class WorkspaceStatusChangedEventConsumer : IConsumer<WorkspaceStatusChangedEvent>
    {
        private readonly IDomainSynchronizer _synchronizer;

        public WorkspaceStatusChangedEventConsumer(IDomainSynchronizer synchronizer)
        {
            _synchronizer = synchronizer;
        }

        public async Task ConsumeAsync(WorkspaceStatusChangedEvent message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
