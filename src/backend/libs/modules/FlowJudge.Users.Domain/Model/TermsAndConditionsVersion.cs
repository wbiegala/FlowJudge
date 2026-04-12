namespace FlowJudge.Users.Domain.Model
{
    public sealed class TermsAndConditionsVersion : DocumentVersion
    {
        internal TermsAndConditionsVersion(Guid id, int number, string textContent, string htmlContent, DateTimeOffset creationTimespamp, bool isAcceptable)
            : base(number, textContent, htmlContent, creationTimespamp, isAcceptable)
        {
            Id = id;
        }

        public static TermsAndConditionsVersion Load(Guid id, int number, string textContent, string htmlContent, DateTimeOffset creationTimespamp, bool isAcceptable) =>
            new TermsAndConditionsVersion(id, number, textContent, htmlContent, creationTimespamp, isAcceptable);
    }
}
