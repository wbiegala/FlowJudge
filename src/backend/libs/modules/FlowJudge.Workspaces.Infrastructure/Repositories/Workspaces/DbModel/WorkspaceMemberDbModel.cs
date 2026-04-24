namespace FlowJudge.Workspaces.Infrastructure.Repositories.Workspaces.DbModel
{
    public sealed record WorkspaceMemberDbModel
    {
        public Guid id { get; init; }
        public Guid workspace_id { get; init; }
        public Guid member_id { get; init; }
        public string role { get; init; }
        public DateTimeOffset assigned_at { get; init; }
        public Guid? assigned_by { get; init; }
    }
}
