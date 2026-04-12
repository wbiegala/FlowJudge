using FlowJudge.Users.Domain.Model;
using FlowJudge.Users.Domain.Services;
using FlowJudge.Users.Infrastructure;

namespace FlowJudge.Users.Application.Services.Domain
{
    internal sealed class UserLegalStateService : IUserLegalStateService
    {
        private readonly IDocumentVersionRepository _documentVersionRepository;

        public UserLegalStateService(IDocumentVersionRepository documentVersionRepository)
        {
            _documentVersionRepository = documentVersionRepository;
        }

        public async Task<UserLegalState> CheckLegalAsync(User user, CancellationToken cancellationToken = default)
        {
            UserTermsAndConditionsState termsState;

            if (user.TermsAndConditionsAcceptance is null)
            {
                termsState = UserTermsAndConditionsState.TemrsMissing;
            }
            else
            {
                var termsAndConditions = await _documentVersionRepository.GetDocumentVersionByVersionNumberAsync<TermsAndConditionsVersion>(
                    user.TermsAndConditionsAcceptance.VersionNumber, cancellationToken);

                if (termsAndConditions is null)
                {
                    termsState = UserTermsAndConditionsState.TemrsMissing;
                }
                else
                {
                    termsState = termsAndConditions.IsAcceptable
                        ? UserTermsAndConditionsState.TermsAccepted
                        : UserTermsAndConditionsState.TermsUnactualAccepted;
                }
            }

            UserPrivacyPolicyState privacyPolicyState;

            if (user.PrivacyPolicyAcceptance is null)
            {
                privacyPolicyState = UserPrivacyPolicyState.PrivacyPolicyMissing;
            }
            else
            {
                var privacyPolicy = await _documentVersionRepository.GetDocumentVersionByVersionNumberAsync<PrivacyPolicyVersion>(
                    user.PrivacyPolicyAcceptance.VersionNumber, cancellationToken);

                if (privacyPolicy is null)
                {
                    privacyPolicyState = UserPrivacyPolicyState.PrivacyPolicyMissing;
                }
                else
                {
                    privacyPolicyState = privacyPolicy.IsAcceptable
                        ? UserPrivacyPolicyState.PrivacyPolicyAccepted
                        : UserPrivacyPolicyState.PrivacyPolicyUnactualAccepted;
                }
            }
            var isLegal = termsState == UserTermsAndConditionsState.TermsAccepted && privacyPolicyState == UserPrivacyPolicyState.PrivacyPolicyAccepted;

            return new UserLegalState(isLegal, termsState, privacyPolicyState);
        }
    }
}
