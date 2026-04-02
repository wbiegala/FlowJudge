using FlowJudge.Common.Sql.Migrations;
using FlowJudge.Users.Infrastructure.Repositories.DocumentVersions;
using Cfg = FlowJudge.Users.Infrastructure.UsersContextConfiguration;

namespace FlowJudge.Users.Infrastructure.Migrations
{

    [Migration(2, "Create DocumentVersion table")]
    internal sealed class Migration002_CreateDocumentVersionTable : IMigration
    {
        public async Task ExecuteAsync(IMigrationContext migrationContext)
        {
            await migrationContext.ExecuteAsync(CreateDocumentVersionTableSql);
            await migrationContext.ExecuteAsync(CreateUniqueIndexOnDocumentVersionNumberSql);
        }

        private const string CreateDocumentVersionTableSql = $@"
CREATE TABLE {Cfg.SchemaName}.{Cfg.DocumentVersionsTableName} (
    {nameof(DocumentVersionDbModel.id)} uuid PRIMARY KEY,
    {nameof(DocumentVersionDbModel.number)} integer not null,
    {nameof(DocumentVersionDbModel.text_content)} text not null,
    {nameof(DocumentVersionDbModel.html_content)} text not null,
    {nameof(DocumentVersionDbModel.creation_timestamp)} timestamptz not null,
    {nameof(DocumentVersionDbModel.is_acceptable)} boolean not null,
    {nameof(DocumentVersionDbModel.document_kind)} text not null,

    constraint chk_document_versions_number_gt_0
        check ({nameof(DocumentVersionDbModel.number)} > 0),

    constraint chk_document_versions_document_kind
        check ({nameof(DocumentVersionDbModel.document_kind)} in ('{nameof(DocumentKindDbModel.TermsAndConditions)}', '{nameof(DocumentKindDbModel.PrivacyPolicy)}'))
);";

        private const string CreateUniqueIndexOnDocumentVersionNumberSql = $@"
create unique index if not exists ux_{Cfg.DocumentVersionsTableName}_{nameof(DocumentVersionDbModel.document_kind)}_{nameof(DocumentVersionDbModel.number)}
    on {Cfg.SchemaName}.{Cfg.DocumentVersionsTableName} ({nameof(DocumentVersionDbModel.document_kind)}, {nameof(DocumentVersionDbModel.number)});";
    }
}
