namespace FlowJudge.GitHub.Webhooks.Security
{
    public interface IGitHubWebhookSignatureValidator
    {
        bool IsSignatureValid(GitHubWebhookMetadata metadata);
    }
}
