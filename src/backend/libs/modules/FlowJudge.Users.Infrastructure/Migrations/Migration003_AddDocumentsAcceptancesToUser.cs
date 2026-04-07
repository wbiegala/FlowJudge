using FlowJudge.Common.Sql.Migrations;
using FlowJudge.Users.Infrastructure.Repositories.Users;
using Cfg = FlowJudge.Users.Infrastructure.UsersContextConfiguration;

namespace FlowJudge.Users.Infrastructure.Migrations
{
    [Migration(3, "Add documents acceptances to user")]
    internal sealed class Migration003_AddDocumentsAcceptancesToUser : IMigration
    {
        public async Task ExecuteAsync(IMigrationContext migrationContext)
        {
            await migrationContext.ExecuteAsync(AddDocumentAcceptancesColumnsToUserTableSql);
        }

        private const string AddDocumentAcceptancesColumnsToUserTableSql = @$"
alter table {Cfg.SchemaName}.{Cfg.UsersTableName}
    add column if not exists {nameof(UserDbModel.terms_accepted_version)} integer null,
    add column if not exists {nameof(UserDbModel.terms_accepted_at)} timestamptz null,
    add column if not exists {nameof(UserDbModel.privacy_policy_accepted_version)} integer null,
    add column if not exists {nameof(UserDbModel.privacy_policy_accepted_at)} timestamptz null;

alter table users.users
    add constraint chk_users_terms_acceptance_consistency
        check (
            ({nameof(UserDbModel.terms_accepted_version)} is null and {nameof(UserDbModel.terms_accepted_at)} is null)
            or
            ({nameof(UserDbModel.terms_accepted_version)} is not null and {nameof(UserDbModel.terms_accepted_at)} is not null)
        ),
    add constraint chk_users_privacy_policy_acceptance_consistency
        check (
            ({nameof(UserDbModel.privacy_policy_accepted_version)} is null and {nameof(UserDbModel.privacy_policy_accepted_at)} is null)
            or
            ({nameof(UserDbModel.privacy_policy_accepted_version)} is not null and {nameof(UserDbModel.privacy_policy_accepted_at)} is not null)
        ),
    add constraint chk_users_{nameof(UserDbModel.terms_accepted_version)}_gt_0
        check (
            {nameof(UserDbModel.terms_accepted_version)} is null or {nameof(UserDbModel.terms_accepted_version)} > 0
        ),
    add constraint chk_users_{nameof(UserDbModel.privacy_policy_accepted_version)}_gt_0
        check (
            {nameof(UserDbModel.privacy_policy_accepted_version)} is null or {nameof(UserDbModel.privacy_policy_accepted_version)} > 0
        );";
    }
}
