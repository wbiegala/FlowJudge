using FlowJudge.Common.Application.Mediator;

namespace FlowJudge.Workspaces.Application.Commands
{
    public sealed record CreateWorkspaceCommand(string Name, Guid CreatorId) : ICommand<Guid>;
}
