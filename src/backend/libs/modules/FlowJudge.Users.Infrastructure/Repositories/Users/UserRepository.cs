using Dapper;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Users.Application.Abstractions.Ports;
using FlowJudge.Users.Domain.Model;

namespace FlowJudge.Users.Infrastructure.Repositories.Users
{
    internal sealed class UserRepository : DapperRepository, IUserRepository
    {
        public UserRepository(ISqlSession sqlSession) : base(sqlSession)
        {
        }

        public async Task AddUserAsync(User user, CancellationToken cancellationToken = default)
        {           
            var model = user.ToDbModel();
            var command = Command(InsertUserCommand, model, cancellationToken);
            await EnsureConnectionOpenAsync(cancellationToken);
            await Connection.ExecuteAsync(command);
        }

        public async Task<User?> GetUserByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default)
        {
            var command = Command(GetUserByIdentityIdQuery, new { IdentityId = identityId }, cancellationToken);
            await EnsureConnectionOpenAsync(cancellationToken);
            var model = await Connection.QuerySingleOrDefaultAsync<UserDbModel>(command);

            if (model is null)
                return null;

            return model.ToDomainModel();
        }

        public async Task UpdateUserAsync(User user, CancellationToken cancellationToken = default)
        {
            var command = Command(UpdateUserCommand, user.ToDbModel(), cancellationToken);
            await EnsureConnectionOpenAsync(cancellationToken);
            await Connection.ExecuteAsync(command);
        }


        private const string InsertUserCommand = $@"
INSERT INTO {UsersContextConfiguration.SchemaName}.{UsersContextConfiguration.UsersTableName} (
     {nameof(UserDbModel.id)}
    ,{nameof(UserDbModel.identity_id)}
    ,{nameof(UserDbModel.username)}
    ,{nameof(UserDbModel.email)}
)
VALUES (
    @{nameof(UserDbModel.id)}
    ,@{nameof(UserDbModel.identity_id)}
    ,@{nameof(UserDbModel.username)}
    ,@{nameof(UserDbModel.email)});";

        private const string GetUserByIdentityIdQuery = $@"
SELECT DISTINCT
     {nameof(UserDbModel.id)}
    ,{nameof(UserDbModel.identity_id)}
    ,{nameof(UserDbModel.username)}
    ,{nameof(UserDbModel.email)}
    ,{nameof(UserDbModel.terms_accepted_version)}
    ,{nameof(UserDbModel.terms_accepted_at)}
    ,{nameof(UserDbModel.privacy_policy_accepted_version)}
    ,{nameof(UserDbModel.privacy_policy_accepted_at)}
FROM {UsersContextConfiguration.SchemaName}.{UsersContextConfiguration.UsersTableName} u
WHERE {nameof(UserDbModel.identity_id)}=@IdentityId;";

        private const string UpdateUserCommand = $@"
UPDATE {UsersContextConfiguration.SchemaName}.{UsersContextConfiguration.UsersTableName} u
SET
     {nameof(UserDbModel.identity_id)} = @{nameof(UserDbModel.identity_id)}
    ,{nameof(UserDbModel.username)} = @{nameof(UserDbModel.username)}
    ,{nameof(UserDbModel.email)} = @{nameof(UserDbModel.email)}
    ,{nameof(UserDbModel.terms_accepted_version)} = @{nameof(UserDbModel.terms_accepted_version)}
    ,{nameof(UserDbModel.terms_accepted_at)} = @{nameof(UserDbModel.terms_accepted_at)}
    ,{nameof(UserDbModel.privacy_policy_accepted_version)} = @{nameof(UserDbModel.privacy_policy_accepted_version)}
    ,{nameof(UserDbModel.privacy_policy_accepted_at)} = @{nameof(UserDbModel.privacy_policy_accepted_at)}
WHERE {nameof(UserDbModel.id)}=@{nameof(UserDbModel.id)}";
    }
}
