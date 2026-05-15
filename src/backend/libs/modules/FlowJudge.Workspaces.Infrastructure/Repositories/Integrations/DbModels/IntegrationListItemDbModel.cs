namespace FlowJudge.Workspaces.Infrastructure.Repositories.Integrations.DbModels
{
    internal sealed record IntegrationListItemDbModel
    {
        public Guid aggregate_id { get; init; }
        public required string name { get; init; }
        public required string provider { get; init; }
        public required string status { get; init; }
        public DateTimeOffset created_at { get; init; }
        public Guid created_by { get; init; }
    }
}
