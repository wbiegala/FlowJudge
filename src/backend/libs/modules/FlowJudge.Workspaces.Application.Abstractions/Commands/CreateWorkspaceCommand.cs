using FlowJudge.Common.Application.Mediator;

namespace FlowJudge.Workspaces.Application.Abstractions.Commands
{
    public sealed record CreateWorkspaceCommand(string Name, Guid CreatorId) : ICommand<Guid>;
}
