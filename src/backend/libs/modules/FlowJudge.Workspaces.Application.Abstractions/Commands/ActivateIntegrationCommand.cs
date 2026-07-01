using FlowJudge.Common.Application.Mediator;

namespace FlowJudge.Workspaces.Application.Abstractions.Commands
{
    public sealed record ActivateIntegrationCommand(
        Guid WorkspaceId,
        Guid IntegrationId,
        Guid IssuerId) : ICommand;
}
