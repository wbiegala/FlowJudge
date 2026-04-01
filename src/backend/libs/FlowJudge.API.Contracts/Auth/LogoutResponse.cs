namespace FlowJudge.API.Contracts.Auth
{
    public sealed record LogoutResponse
    {
        public required string LogoutRedirectUrl { get; init; }
    }
}
