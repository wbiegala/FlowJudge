namespace FlowJudge.API.Contracts.Workspaces
{
    public sealed record UpdateWorkspaceRequest
    {
        public required string Name { get; init; }
    }
}
