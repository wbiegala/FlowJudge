using FlowJudge.Users.Application.Models;

namespace FlowJudge.Users.Application.Queries
{
    public sealed record GetActualDocumentVersionQueryResult
    {
        public DocumentKind Kind { get; init; }
        public Guid VersionId { get; init; }
        public int VersionNumber { get; init; }
        public required string Content { get; init; }
    }
}
