using FlowJudge.Common.Domain;

namespace FlowJudge.Users.Domain.Model
{
    public sealed record TermsAndConditionsAcceptance : ValueObject
    {
        public int VersionNumber { get; }
        public DateTimeOffset AcceptanceTimestamp { get; }

        private TermsAndConditionsAcceptance(int versionNumber, DateTimeOffset acceptanceTimestamp)
        {
            VersionNumber = versionNumber;
            AcceptanceTimestamp = acceptanceTimestamp;
        }

        public static TermsAndConditionsAcceptance Create(int versionNumber, DateTimeOffset acceptanceTimestamp)
        {
            return new TermsAndConditionsAcceptance(versionNumber, acceptanceTimestamp);
        }
    }
}
