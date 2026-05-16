using FlowJudge.Common.Application.Mediator;
using FlowJudge.Workspaces.Domain.Integration.Model;

namespace FlowJudge.Workspaces.Application.Abstractions.Commands
{
    public sealed record CreateIntegrationCommand(
        string Name,
        IntegrationProvider Provider,
        Guid WorkspaceId,
        Guid IssuerId) : ICommand<Guid>;
}
