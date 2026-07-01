namespace FlowJudge.GitHub.Webhooks
{
    public sealed record GitHubWebhookMetadata
    {
        public long WebhookId { get; init; }
        public Guid DeliveryId { get; init; }
        public GitHubEventType Type { get; init; }
        public DateTimeOffset ReceivedAt { get; init; }
        public SignatureHashType SignatureHash { get; init; }
        public required string Signature { get; init; }
        public string? TargetType { get; init; }
        public string? TargetId { get; init; }
        public required string SerializedContent { get; init; }
    }
}
