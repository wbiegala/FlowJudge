namespace FlowJudge.API.Contracts.Auth
{
    public sealed record TokenPairResponse
    {
        public required string AccessToken { get; init; }
        public required string IdentityToken { get; init; }
    }
}
