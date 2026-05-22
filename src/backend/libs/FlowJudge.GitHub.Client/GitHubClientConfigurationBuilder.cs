namespace FlowJudge.GitHub.Client
{
    public sealed class GitHubClientConfigurationBuilder
    {
        private string _baseUrl = "https://github.com";
        private string _baseApiUrl = "https://api.github.com";
        private string _applicationName = "FlowJudge";
        private string _apiVersion = "2026-03-10";
        private (string appId, string pk)? _installationTokenAuthConfig;

        public void WithBaseUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        public void WithBaseApiUrl(string baseApiUrl)
        {
            _baseApiUrl = baseApiUrl;
        }

        public void WithApiVersion(string apiVersion)
        {
            _apiVersion = apiVersion;
        }

        public void WithApplicationName(string applicationName)
        {
            _applicationName = applicationName;
        }

        public void WithInstallationTokenAuth(string applicationId, string privateKey)
        {
            _installationTokenAuthConfig = (applicationId, privateKey);
        }

        internal GitHubClientConfiguration Build()
        {
            return new GitHubClientConfiguration { 
                BaseUrl = _baseUrl,
                BaseApiUrl = _baseApiUrl,
                ApplicationName = _applicationName,
                ApiVersion = _apiVersion,
                InstallationTokenAuthConfiguration = _installationTokenAuthConfig.HasValue
                    ? new GitHubClientConfiguration.GitHubInstallationTokenAuthConfiguration
                        {
                            ApplicationId = _installationTokenAuthConfig.Value.appId,
                            PrivateKey = _installationTokenAuthConfig.Value.pk,
                        }
                    : null
            };
        }
    }
}
