namespace FlowJudge.Workspaces.Infrastructure.Repositories.Integrations.DbModels
{
    internal sealed record IntegrationAuthenticationDbModel
    {
        public Guid id { get; init; }
        public Guid integration_id { get; init; }
        public required string type { get; init; }
        public required string status { get; init; }
        public required string value { get; init; }
        public DateTimeOffset? valid_to { get; init; }
        public DateTimeOffset created_at { get; init; }
        public Guid created_by { get; init; }
    }
}
