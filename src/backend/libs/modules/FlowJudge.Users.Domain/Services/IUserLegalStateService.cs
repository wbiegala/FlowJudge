using FlowJudge.Users.Domain.Model;

namespace FlowJudge.Users.Domain.Services
{
    public interface IUserLegalStateService
    {
        Task<UserLegalState> CheckLegalAsync(User user, CancellationToken cancellationToken = default);
    }

    public sealed record UserLegalState(bool IsLegal, UserTermsAndConditionsState TermsAndConditionsState, UserPrivacyPolicyState PrivacyPolicyState);

    public enum UserTermsAndConditionsState
    {
        TermsAccepted,
        TermsUnactualAccepted,
        TemrsMissing
    }

    public enum UserPrivacyPolicyState
    {
        PrivacyPolicyAccepted,
        PrivacyPolicyUnactualAccepted,
        PrivacyPolicyMissing
    }
}
