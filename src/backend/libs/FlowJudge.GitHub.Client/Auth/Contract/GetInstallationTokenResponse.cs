namespace FlowJudge.GitHub.Client.Auth.Contract
{
    internal sealed record GetInstallationTokenResponse
    {
        public required string token { get; init; }
        public DateTimeOffset expires_at { get; init; }
    }
}
