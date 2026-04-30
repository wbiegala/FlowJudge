using FlowJudge.Workspaces.Domain.Workspace.Model;

namespace FlowJudge.Workspaces.Domain.Workspace.ReadModels
{
    public sealed record WorkspaceListItem
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public Guid OwnerId { get; init; }
        public WorkspaceRole Role { get; init; }
        public DateTimeOffset CreatedAt { get; init; }
    }
}
