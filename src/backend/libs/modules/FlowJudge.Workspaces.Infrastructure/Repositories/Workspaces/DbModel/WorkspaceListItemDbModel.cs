namespace FlowJudge.Workspaces.Infrastructure.Repositories.Workspaces.DbModel
{
    public sealed record WorkspaceListItemDbModel
    {
        public Guid workspace_id { get; init; }
        public string name { get; init; }
        public Guid owner_id { get; init; }
        public string role { get; init; }
        public DateTimeOffset created_at { get; init; }
    }
}
