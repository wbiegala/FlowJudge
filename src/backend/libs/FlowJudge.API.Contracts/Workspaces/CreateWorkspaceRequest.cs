namespace FlowJudge.API.Contracts.Workspaces
{
    public sealed record CreateWorkspaceRequest
    {
        public required string Name { get; init; }
    }
}
