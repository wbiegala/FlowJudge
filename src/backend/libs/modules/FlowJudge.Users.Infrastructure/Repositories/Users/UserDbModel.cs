using FlowJudge.Users.Domain.Model;

namespace FlowJudge.Users.Infrastructure.Repositories.Users
{
    internal sealed record UserDbModel
    {
        public Guid id { get; init; }
        public Guid identity_id { get; init; }
        public string username { get; init; } = null!;
        public string email { get; init; } = null!;
    }

    internal static class UserDbModelExtensions
    {
        public static UserDbModel ToDbModel(this User user)
        {
            return new UserDbModel
            {
                id = user.Id,
                identity_id = user.IdentityId,
                username = user.UserName,
                email = user.EmailAddress
            };
        }

        public static User ToDomainModel(this UserDbModel dbModel)
        {
            return User.Load(dbModel.id, dbModel.identity_id, dbModel.username, dbModel.email);
        }
    }
}
