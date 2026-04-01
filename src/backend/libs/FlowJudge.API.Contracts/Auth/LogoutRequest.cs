namespace FlowJudge.API.Contracts.Auth
{
    public sealed record LogoutRequest
    {
        public required string IdentityToken { get; init; }
        public required string UiContext { get; init; }
    }
}
