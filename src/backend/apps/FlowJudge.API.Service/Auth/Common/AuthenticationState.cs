namespace FlowJudge.API.Service.Auth.Common
{
    internal sealed record AuthenticationState
    {
        public Guid Id { get; init; }
        public required string UiContextUrl { get; init; }
        public required string RedirectUri { get; init; }
        public DateTimeOffset CreatedAt { get; init; }

        public string? AccessToken { get; set; }
        public string? IdentityToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
