using FlowJudge.Common.Application.Mediator;

namespace FlowJudge.Workspaces.Application.Abstractions.Commands
{
    public sealed record DeactivateIntegrationCommand(
        Guid WorkspaceId,
        Guid IntegrationId,
        Guid IssuerId) : ICommand;
}
