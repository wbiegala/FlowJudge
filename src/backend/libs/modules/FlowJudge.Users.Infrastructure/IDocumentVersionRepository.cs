using FlowJudge.Users.Domain.Model;

namespace FlowJudge.Users.Infrastructure
{
    public interface IDocumentVersionRepository
    {
        Task<DocumentVersion?> GetDocumentVersionByIdAsync(Guid versionId, CancellationToken cancellationToken = default);
        Task<TDocumentVersion?> GetActualDocumentVersionAsync<TDocumentVersion>(CancellationToken cancellationToken)
            where TDocumentVersion : DocumentVersion;
    }
}
