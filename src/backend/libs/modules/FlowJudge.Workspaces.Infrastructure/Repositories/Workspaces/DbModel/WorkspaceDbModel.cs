namespace FlowJudge.Workspaces.Infrastructure.Repositories.Workspaces.DbModel
{
    public sealed record WorkspaceDbModel
    {
        public Guid id { get; init; }
        public Guid aggregate_id { get; init; }
        public string name { get; init; }
        public string status { get; init; }
        public DateTimeOffset created_at { get; init; }
        public Guid created_by { get; init; }
    }
}
