namespace FlowJudge.API.Contracts.Integrations
{
    public sealed record CreateIntegrationRequest
    {
        public required string Name { get; init; }
    }
}
