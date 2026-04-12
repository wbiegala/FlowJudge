using FlowJudge.Common.Domain;

namespace FlowJudge.Users.Domain.Model
{
    public sealed record PrivacyPolicyAcceptance : ValueObject
    {
        public int VersionNumber { get; }
        public DateTimeOffset AcceptanceTimestamp { get; }

        private PrivacyPolicyAcceptance(int versionNumber, DateTimeOffset acceptanceTimestamp)
        {
            VersionNumber = versionNumber;
            AcceptanceTimestamp = acceptanceTimestamp;
        }

        public static PrivacyPolicyAcceptance Create(int versionNumber, DateTimeOffset acceptanceTimestamp)
        {
            return new PrivacyPolicyAcceptance(versionNumber, acceptanceTimestamp);
        }
    }
}
