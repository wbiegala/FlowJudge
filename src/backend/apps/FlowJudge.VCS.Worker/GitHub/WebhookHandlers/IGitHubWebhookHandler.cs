using FlowJudge.GitHub.Webhooks;

namespace FlowJudge.VCS.Worker.GitHub.WebhooksServices
{
    public interface IGitHubWebhookHandler
    {
        bool CanHandle(GitHubEventType type);

        Task HandleAsync(GitHubWebhookMetadata webhook, CancellationToken ct = default);
    }
}
