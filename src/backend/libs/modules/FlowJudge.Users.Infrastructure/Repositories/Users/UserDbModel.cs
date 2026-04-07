using FlowJudge.Users.Domain.Model;

namespace FlowJudge.Users.Infrastructure.Repositories.Users
{
    internal sealed record UserDbModel
    {
        public Guid id { get; init; }
        public Guid identity_id { get; init; }
        public string username { get; init; } = null!;
        public string email { get; init; } = null!;

        public int? terms_accepted_version { get; init; }
        public DateTimeOffset? terms_accepted_at { get; init; }
        public int? privacy_policy_accepted_version { get; init; }
        public DateTimeOffset? privacy_policy_accepted_at { get; init; }
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
                email = user.EmailAddress,
                terms_accepted_version = user.TermsAndConditionsAcceptance?.VersionNumber,
                terms_accepted_at = user.TermsAndConditionsAcceptance?.AcceptanceTimestamp,
                privacy_policy_accepted_version = user.PrivacyPolicyAcceptance?.VersionNumber,
                privacy_policy_accepted_at = user.PrivacyPolicyAcceptance?.AcceptanceTimestamp
            };
        }

        public static User ToDomainModel(this UserDbModel dbModel)
        {
            return User.Load(
                dbModel.id,
                dbModel.identity_id,
                dbModel.username,
                dbModel.email,
                dbModel.terms_accepted_version,
                dbModel.terms_accepted_at,
                dbModel.privacy_policy_accepted_version,
                dbModel.privacy_policy_accepted_at);
        }
    }
}
