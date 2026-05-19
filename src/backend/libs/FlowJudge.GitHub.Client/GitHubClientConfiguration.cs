namespace FlowJudge.GitHub.Client
{
    internal sealed record GitHubClientConfiguration
    {
        public required string BaseApiUrl { get; init; }
        public required string BaseUrl { get; init; }
        public required string ApplicationName { get; init; }
        public required string ApiVersion { get; init; }
        public GitHubInstallationTokenAuthConfiguration? InstallationTokenAuthConfiguration { get; init; }

        internal sealed record GitHubInstallationTokenAuthConfiguration
        {
            public required string ApplicationId { get; init; }
            public required string PrivateKey { get; init; }
        }
    }
}
