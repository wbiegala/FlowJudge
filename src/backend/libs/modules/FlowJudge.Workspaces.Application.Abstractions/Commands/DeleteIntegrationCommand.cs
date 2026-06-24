using FlowJudge.Common.Application.Mediator;

namespace FlowJudge.Workspaces.Application.Abstractions.Commands
{
    public sealed record DeleteIntegrationCommand(
        Guid WorkspaceId,
        Guid IntegrationId,
        Guid IssuerId) : ICommand;
}
