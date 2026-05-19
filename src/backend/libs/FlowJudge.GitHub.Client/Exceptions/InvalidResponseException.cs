namespace FlowJudge.GitHub.Client.Exceptions
{
    public sealed class InvalidResponseException : GitHubClientException
    {
        public InvalidResponseException(string endpoint, Type expectedType)
            : base(endpoint, $"Invalid response content - expected type {expectedType.Name}.") { }

        public InvalidResponseException(string installationId, string endpoint, Type expectedType)
            : base(endpoint, $"Invalid response content for installationId={installationId} - expected type {expectedType.Name}.") { }
    }
}
