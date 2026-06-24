namespace FlowJudge.Workspaces.Infrastructure.Repositories.Repositories.DbModels
{
    internal sealed record RepositoryDbModel
    {
        public Guid id { get; init; }
        public Guid aggregate_id { get; init; }
        public Guid workspace_id { get; init; }
        public Guid integration_id { get; init; }
        public required string vcs_external_id { get; init; }
        public required string name { get; init; }
        public string? full_name { get; init; }
        public bool is_tracking { get; init; }
        public required string status { get; init; }
    }
}
