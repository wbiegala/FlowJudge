using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Users.Application.Abstractions.Ports;
using FlowJudge.Users.Application.Abstractions.Queries;
using FlowJudge.Users.Application.Extensions;
using FlowJudge.Users.Application.Models;
using FlowJudge.Users.Domain.Model;

namespace FlowJudge.Users.Application.Queries
{
    internal sealed class GetActualDocumentVersionQueryHandler : IQueryHandler<GetActualDocumentVersionQuery, GetActualDocumentVersionQueryResult>
    {
        private readonly IDocumentVersionRepository _documentVersionRepository;

        public GetActualDocumentVersionQueryHandler(IDocumentVersionRepository documentVersionRepository)
        {
            _documentVersionRepository = documentVersionRepository;
        }

        public async Task<IResult<GetActualDocumentVersionQueryResult>> HandleAsync(
            GetActualDocumentVersionQuery query,
            CancellationToken cancellationToken = default)
        {
            var documentVersion = await GetActualDocumentVersionAsync(query.Kind, cancellationToken);

            if (documentVersion is null)
                return ApplicationResultFactory.Failure<GetActualDocumentVersionQueryResult>(
                    $"Actual document version with kind={query.Kind.ToString()} not found",
                    ErrorCodeGenerator.NotFound("document_version"),
                    new Dictionary<string, object> { { "kind", query.Kind } });

            return ApplicationResultFactory.Success(new GetActualDocumentVersionQueryResult
            {
                Kind = query.Kind,
                VersionId = documentVersion.Id,
                VersionNumber = documentVersion.Number,
                Content = query.ContentType switch
                {
                    DocumentContentType.Text => documentVersion.TextContent,
                    DocumentContentType.Html => documentVersion.HtmlContent,
                    _ => throw new InvalidOperationException($"Unsupported content type: {query.ContentType}")
                }
            });
        }

        private async Task<DocumentVersion?> GetActualDocumentVersionAsync(DocumentKind kind, CancellationToken cancellationToken)
        {
            switch (kind)
            {
                case DocumentKind.TermsAndConditions:
                    return await _documentVersionRepository.GetActualTermsAndConditionsVersionAsync(cancellationToken);
                case DocumentKind.PrivacyPolicy:
                    return await _documentVersionRepository.GetActualPrivacyPolicyVersionAsync(cancellationToken);
                default:
                    return null;
            }
        }
    }
}
