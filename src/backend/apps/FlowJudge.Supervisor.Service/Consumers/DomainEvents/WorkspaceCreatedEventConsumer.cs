using FlowJudge.Common.Messaging;
using FlowJudge.Reviews.Application.Abstractions.Ports;
using FlowJudge.Workspaces.Domain.Events.Workspace;

namespace FlowJudge.Supervisor.Service.Consumers.DomainEvents
{
    public class WorkspaceCreatedEventConsumer : IConsumer<WorkspaceCreatedEvent>
    {
        private readonly IDomainSynchronizer _synchronizer;

        public WorkspaceCreatedEventConsumer(IDomainSynchronizer synchronizer)
        {
            _synchronizer = synchronizer;
        }

        public async Task ConsumeAsync(WorkspaceCreatedEvent message, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
