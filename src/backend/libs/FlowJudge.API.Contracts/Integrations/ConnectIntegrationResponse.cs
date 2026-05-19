namespace FlowJudge.API.Contracts.Integrations
{
    public sealed record ConnectIntegrationResponse
    {
        public required string RedirectUrl { get; init; }
    }
}
