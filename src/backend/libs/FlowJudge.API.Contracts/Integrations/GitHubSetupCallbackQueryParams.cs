namespace FlowJudge.API.Contracts.Integrations
{
    public sealed record GitHubSetupCallbackQueryParams
    {
        public required string state { get; init; }
        public required string installation_id { get; init; }
        public string? setup_action { get; init; }
    }
}
