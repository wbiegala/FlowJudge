using FlowJudge.Common.Application.Mediator;

namespace FlowJudge.Workspaces.Application.Abstractions.Commands
{
    public sealed record UpdateWorkspaceCommand(
        Guid WorkspaceId,
        string Name,
        Guid IssuerId) : ICommand;
}
