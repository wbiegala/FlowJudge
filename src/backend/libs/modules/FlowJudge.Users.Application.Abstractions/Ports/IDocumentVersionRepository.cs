using FlowJudge.Users.Domain.Model;

namespace FlowJudge.Users.Application.Abstractions.Ports
{
    public interface IDocumentVersionRepository
    {
        Task<DocumentVersion?> GetDocumentVersionByIdAsync(Guid versionId, CancellationToken cancellationToken = default);
        Task<TDocumentVersion?> GetActualDocumentVersionAsync<TDocumentVersion>(CancellationToken cancellationToken = default)
            where TDocumentVersion : DocumentVersion;
        Task<DocumentVersion?> GetDocumentVersionByVersionNumberAsync<TDocumentVersion>(int number, CancellationToken cancellationToken = default)
            where TDocumentVersion : DocumentVersion;
    }
}
