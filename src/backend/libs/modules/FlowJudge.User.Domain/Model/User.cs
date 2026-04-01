using FlowJudge.Common.Domain;
using FlowJudge.Shared.Domain.ValueObjects;

namespace FlowJudge.User.Domain.Model
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
    }
}
