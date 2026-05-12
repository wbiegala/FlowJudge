using Dapper;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Users.Application.Abstractions.Ports;
using FlowJudge.Users.Domain.Model;

namespace FlowJudge.Users.Infrastructure.Repositories.DocumentVersions
{
    internal sealed class DocumentVersionRepository : DapperRepository, IDocumentVersionRepository
    {
        public DocumentVersionRepository(ISqlSession sqlSession)
            : base(sqlSession) { }

        public async Task<DocumentVersion?> GetDocumentVersionByIdAsync(
            Guid versionId,
            CancellationToken cancellationToken = default)
        {
            await EnsureConnectionOpenAsync(cancellationToken);
            var command = Command(GetDocumentVersionByIdSql, new { VersionId = versionId }, cancellationToken);
            var documentVersion = await Connection.QuerySingleOrDefaultAsync<DocumentVersionDbModel>(command);

            if (documentVersion is null)
                return null;

            return documentVersion.ToDomainModel();
        }

        public async Task<TDocumentVersion?> GetActualDocumentVersionAsync<TDocumentVersion>(CancellationToken cancellationToken = default)
            where TDocumentVersion : DocumentVersion
        {
            var type = typeof(TDocumentVersion);
            var documentKind = type == typeof(PrivacyPolicyVersion) ? DocumentKindDbModel.PrivacyPolicy :
                               type == typeof(TermsAndConditionsVersion) ? DocumentKindDbModel.TermsAndConditions :
                               throw new InvalidOperationException($"Unsupported document version type: {type.Name}");
            await EnsureConnectionOpenAsync(cancellationToken);
            var command = Command(GetAllActualDocumentVersionsSql, new { DocumentKind = documentKind.ToString() }, cancellationToken);
            var versions = await Connection.QueryAsync<DocumentVersionDbModel>(command);

            var documentVersion = versions
                .OrderByDescending(v => v.number)
                .FirstOrDefault();

            if (documentVersion is null)
                return null;

            return documentVersion.ToDomainModel() as TDocumentVersion;
        }

        public async Task<DocumentVersion?> GetDocumentVersionByVersionNumberAsync<TDocumentVersion>(
            int number,
            CancellationToken cancellationToken = default)
                where TDocumentVersion : DocumentVersion
        {
            var type = typeof(TDocumentVersion);
            var documentKind = type == typeof(PrivacyPolicyVersion) ? DocumentKindDbModel.PrivacyPolicy :
                               type == typeof(TermsAndConditionsVersion) ? DocumentKindDbModel.TermsAndConditions :
                               throw new InvalidOperationException($"Unsupported document version type: {type.Name}");
            await EnsureConnectionOpenAsync(cancellationToken);
            var command = Command(GetDocumentVersionByNumverSql, new { DocumentKind = documentKind.ToString(), VersionNumber = number }, cancellationToken);
            var documentVersion = await Connection.QueryFirstOrDefaultAsync<DocumentVersionDbModel>(command);

            if (documentVersion is null)
                return null;

            return documentVersion.ToDomainModel() as TDocumentVersion;
        }

        private const string GetDocumentVersionByIdSql = $@"
SELECT DISTINCT
     {nameof(DocumentVersionDbModel.id)}
    ,{nameof(DocumentVersionDbModel.number)}
    ,{nameof(DocumentVersionDbModel.text_content)}
    ,{nameof(DocumentVersionDbModel.html_content)}
    ,{nameof(DocumentVersionDbModel.creation_timestamp)}
    ,{nameof(DocumentVersionDbModel.is_acceptable)}
    ,{nameof(DocumentVersionDbModel.document_kind)}
FROM {UsersContextConfiguration.SchemaName}.{UsersContextConfiguration.DocumentVersionsTableName} dv
WHERE {nameof(DocumentVersionDbModel.id)}=@VersionId;";

        private const string GetAllActualDocumentVersionsSql = $@"
SELECT DISTINCT
         {nameof(DocumentVersionDbModel.id)}
        ,{nameof(DocumentVersionDbModel.number)}
        ,{nameof(DocumentVersionDbModel.text_content)}
        ,{nameof(DocumentVersionDbModel.html_content)}
        ,{nameof(DocumentVersionDbModel.creation_timestamp)}
        ,{nameof(DocumentVersionDbModel.is_acceptable)}
        ,{nameof(DocumentVersionDbModel.document_kind)}
FROM {UsersContextConfiguration.SchemaName}.{UsersContextConfiguration.DocumentVersionsTableName} dv
WHERE {nameof(DocumentVersionDbModel.is_acceptable)}=true AND {nameof(DocumentVersionDbModel.document_kind)}=@DocumentKind;
";

        private const string GetDocumentVersionByNumverSql = $@"
SELECT DISTINCT
     {nameof(DocumentVersionDbModel.id)}
    ,{nameof(DocumentVersionDbModel.number)}
    ,{nameof(DocumentVersionDbModel.text_content)}
    ,{nameof(DocumentVersionDbModel.html_content)}
    ,{nameof(DocumentVersionDbModel.creation_timestamp)}
    ,{nameof(DocumentVersionDbModel.is_acceptable)}
    ,{nameof(DocumentVersionDbModel.document_kind)}
FROM {UsersContextConfiguration.SchemaName}.{UsersContextConfiguration.DocumentVersionsTableName} dv
WHERE {nameof(DocumentVersionDbModel.document_kind)}=@DocumentKind AND {nameof(DocumentVersionDbModel.number)}=@VersionNumber;";
    }
}
