using Microsoft.AspNetCore.Http;

namespace FlowJudge.GitHub.Webhooks
{
    public static class GitHubWebhookMetadataExtractor
    {
        private const string WebhookIdHeaderName = "X-GitHub-Hook-ID";
        private const string WebhookEventHeaderName = "X-GitHub-Event";
        private const string WebhookDeliveryHeaderName = "X-GitHub-Delivery";
        private const string WebhookSignatureHeaderName = "X-Hub-Signature";
        private const string WebhookSignature256HeaderName = "X-Hub-Signature-256";
        private const string InstallationTargetTypeHeaderName = "X-GitHub-Hook-Installation-Target-Type";
        private const string InstallationTargetIdHeaderName = "X-GitHub-Hook-Installation-Target-ID";

        public static async Task<GitHubWebhookMetadata?> ExtractAsync(
            HttpRequest request,
            CancellationToken cancellationToken = default)
        {
            var webhookId = request.Headers[WebhookIdHeaderName];
            var webhookEvent = request.Headers[WebhookEventHeaderName];
            var webhookDelivery = request.Headers[WebhookDeliveryHeaderName];
            var webhookSignature = request.Headers[WebhookSignatureHeaderName];
            var webhookSignature256 = request.Headers[WebhookSignature256HeaderName];
            var webhookInstallationTargetType = request.Headers[InstallationTargetTypeHeaderName];
            var webhookInstallationTargetId = request.Headers[InstallationTargetIdHeaderName];
            

            if (string.IsNullOrWhiteSpace(webhookId)
                || string.IsNullOrWhiteSpace(webhookEvent)
                || string.IsNullOrWhiteSpace(webhookDelivery))
                return null;

            if (string.IsNullOrWhiteSpace(webhookSignature) && string.IsNullOrWhiteSpace(webhookSignature256))
                return null;

            var signatureHash = string.IsNullOrWhiteSpace(webhookSignature256) ? SignatureHashType.SHA1 : SignatureHashType.SHA256;
            var signature = string.IsNullOrWhiteSpace(webhookSignature256) ? webhookSignature : webhookSignature256;

            using var reader = new StreamReader(request.Body);
            var content = await reader.ReadToEndAsync(cancellationToken);

            return new GitHubWebhookMetadata
            {
                WebhookId = long.Parse(webhookId!),
                DeliveryId = Guid.Parse(webhookDelivery!),
                Type = ParseGitHubWebhookEventType(webhookEvent),
                ReceivedAt = DateTimeOffset.UtcNow,
                SignatureHash = signatureHash,
                Signature = signature!,
                TargetType = webhookInstallationTargetType,
                TargetId = webhookInstallationTargetId,
                SerializedContent = content,
            };
        }


        public static GitHubEventType ParseGitHubWebhookEventType(string? value)
        {
            return value switch
            {
                "ping" => GitHubEventType.Ping,
                "installation" => GitHubEventType.Installation,
                "installation_repositories" => GitHubEventType.InstallationRepositories,
                "pull_request" => GitHubEventType.PullRequest,
                "pull_request_review" => GitHubEventType.PullRequestReview,
                "pull_request_review_comment" => GitHubEventType.PullRequestReviewComment,
                "check_run" => GitHubEventType.CheckRun,
                "check_suite" => GitHubEventType.CheckSuite,
                "push" => GitHubEventType.Push,
                _ => GitHubEventType.Unknown
            };
        }
    }
}
