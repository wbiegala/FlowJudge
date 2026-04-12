namespace FlowJudge.API.Contracts.Legal
{
    public sealed record GetDocumentVersionResponse
    {
        public required string Kind { get; init; }
        public Guid VersionId { get; init; }
        public int VersionNumber { get; init; }
        public required string Content { get; init; }
    }
}
