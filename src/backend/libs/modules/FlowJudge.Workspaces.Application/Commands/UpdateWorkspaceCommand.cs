using FlowJudge.Common.Application.Mediator;

namespace FlowJudge.Workspaces.Application.Commands
{
    public sealed record UpdateWorkspaceCommand : ICommand
    {
        public required Guid WorkspaceId { get; init; }
        public required string Name { get; init; }
        public required Guid IssuerId { get; init; }
    }
}
