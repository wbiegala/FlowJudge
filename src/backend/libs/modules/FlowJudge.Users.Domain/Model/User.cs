using FlowJudge.Common.Domain;
using FlowJudge.Shared.Domain.ValueObjects;
using FlowJudge.Users.Domain.Model.Exceptions;

namespace FlowJudge.Users.Domain.Model
{
    /// <summary>
    /// Represents user one to one with identity user (from Keycloak)
    /// </summary>
    public sealed class User : AggregateRoot
    {
        /// <summary>
        /// Keycloak user id, which is the same as the user id in our system. We use this to link the user in our system with the user in Keycloak.
        /// </summary>
        public UserIdentityId IdentityId { get; private set; }

        /// <summary>
        /// Keycloak user name
        /// </summary>
        public UserName UserName { get; private set; }

        /// <summary>
        /// Keycloak user email address
        /// </summary>
        public EmailAddress EmailAddress { get; private set; }

        public PrivacyPolicyAcceptance? PrivacyPolicyAcceptance { get; private set; }

        public TermsAndConditionsAcceptance? TermsAndConditionsAcceptance { get; private set; }

        public void AcceptPrivacyPolicy(int versionNumber, DateTimeOffset acceptanceTimestamp)
        {
            var potentialAcceptance = PrivacyPolicyAcceptance.Create(versionNumber, acceptanceTimestamp);

            if (PrivacyPolicyAcceptance is null || this.PrivacyPolicyAcceptance.VersionNumber < potentialAcceptance.VersionNumber)
            {
                PrivacyPolicyAcceptance = potentialAcceptance;
            }

            throw new DocumentVersionNumberException(ErrorCodes.InvalidPrivacyPolicyVersion);
        }

        public void AcceptTermsAndConditions(int versionNumber, DateTimeOffset acceptanceTimestamp)
        {
            var potentialAcceptance = TermsAndConditionsAcceptance.Create(versionNumber, acceptanceTimestamp);

            if (TermsAndConditionsAcceptance is null || this.TermsAndConditionsAcceptance.VersionNumber < potentialAcceptance.VersionNumber)
            {
                TermsAndConditionsAcceptance = potentialAcceptance;
            }

            throw new DocumentVersionNumberException(ErrorCodes.InvalidTermsAndConditionsVersion);
        }

        public static User Create(UserIdentityId identityId, UserName userName, EmailAddress emailAddress)
        {
            var user = new User
            {
                IdentityId = identityId,
                UserName = userName,
                EmailAddress = emailAddress
            };

            return user;
        }

        public static User Load(
            Guid id,
            Guid identityId,
            string username,
            string email,
            int? termsAcceptedVersionNumber,
            DateTimeOffset? termsAcceptedTimestamp,
            int? privacyPolicyAcceptedVersionNumber,
            DateTimeOffset? privacyPolicyAcceptedTimestamp)
        {
            return new User
            {
                Id = id,
                IdentityId = UserIdentityId.Create(identityId),
                UserName = UserName.Create(username),
                EmailAddress = EmailAddress.Create(email),
                TermsAndConditionsAcceptance = (termsAcceptedVersionNumber.HasValue && termsAcceptedTimestamp.HasValue)
                    ? TermsAndConditionsAcceptance.Create(termsAcceptedVersionNumber.Value, termsAcceptedTimestamp.Value)
                    : null,
                PrivacyPolicyAcceptance = (privacyPolicyAcceptedVersionNumber.HasValue && privacyPolicyAcceptedTimestamp.HasValue)
                    ? PrivacyPolicyAcceptance.Create(privacyPolicyAcceptedVersionNumber.Value, privacyPolicyAcceptedTimestamp.Value)
                    : null
            };
        }

        private static class ErrorCodes
        {
            public const string InvalidTermsAndConditionsVersion = "user.invalid_terms_version";
            public const string InvalidPrivacyPolicyVersion = "user.invalid_private_policy_version";
        }
    }
}
