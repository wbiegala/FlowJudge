namespace FlowJudge.Users.Domain.Model
{
    public sealed class PrivacyPolicyVersion : DocumentVersion
    {
        internal PrivacyPolicyVersion(Guid id, int number, string textContent, string htmlContent, DateTimeOffset creationTimespamp, bool isAcceptable)
            : base(number, textContent, htmlContent, creationTimespamp, isAcceptable)
        {
            Id = id;
        }

        public static PrivacyPolicyVersion Load(Guid id, int number, string textContent, string htmlContent, DateTimeOffset creationTimespamp, bool isAcceptable) =>
            new PrivacyPolicyVersion(id, number, textContent, htmlContent, creationTimespamp, isAcceptable);
    }
}
