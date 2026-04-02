using FlowJudge.Users.Domain.Model;

namespace FlowJudge.Users.UnitTests.Application
{
    internal static class Fixture
    {
        public static User CreateUser(Guid id, Guid identityId, string username, string email) =>
            User.Load(id, identityId, username, email);

        public static TermsAndConditionsVersion CreateTermsAndConditionsVersion(
            Guid id,
            int number,
            string textContent,
            string htmlContent,
            DateTimeOffset creationTimestamp,
            bool isAcceptable) =>
            TermsAndConditionsVersion.Load(id, number, textContent, htmlContent, creationTimestamp, isAcceptable);

        public static PrivacyPolicyVersion CreatePrivacyPolicyVersion(
            Guid id,
            int number,
            string textContent,
            string htmlContent,
            DateTimeOffset creationTimestamp,
            bool isAcceptable) =>
            PrivacyPolicyVersion.Load(id, number, textContent, htmlContent, creationTimestamp, isAcceptable);
    }
}
