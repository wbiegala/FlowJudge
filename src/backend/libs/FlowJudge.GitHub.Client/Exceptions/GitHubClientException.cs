namespace FlowJudge.GitHub.Client.Exceptions
{
    public class GitHubClientException : Exception
    {
        private const string DefaultMessage = "Error in GitHub Client";

        public string? Endpoint { get; }

        internal GitHubClientException()
            : base(DefaultMessage) { }
        internal GitHubClientException(string customMessage)
            : base($"{DefaultMessage}. {customMessage}") { }
        internal GitHubClientException(string endpoint, string customMessage)
            : base($"{DefaultMessage} on processing request: {endpoint}. {customMessage}") {}
    }
}
