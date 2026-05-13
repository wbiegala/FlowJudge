using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Users.Application.Abstractions.Ports;
using FlowJudge.Users.Application.Abstractions.Queries;
using FlowJudge.Users.Application.Models;
using FlowJudge.Users.Domain.Services;

namespace FlowJudge.Users.Application.Queries
{
    internal sealed class GetUserLegalStateQueryHandler : IQueryHandler<GetUserLegalStateQuery, GetUserLegalStateQueryResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserLegalStateService _userLegalStateService;

        public GetUserLegalStateQueryHandler(
            IUserRepository userRepository,
            IUserLegalStateService userLegalStateService)
        {
            _userRepository = userRepository;
            _userLegalStateService = userLegalStateService;
        }

        public async Task<IResult<GetUserLegalStateQueryResult>> HandleAsync(GetUserLegalStateQuery query, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByIdentityIdAsync(query.UserIdentityId, cancellationToken);

            if (user is null)
            {
                return ApplicationResultFactory.Failure<GetUserLegalStateQueryResult>(
                    $"User with given identityId={query.UserIdentityId} not found.",
                    ErrorCodeGenerator.NotFound(nameof(user)));
            }

            var legalState = await _userLegalStateService.CheckLegalAsync(user, cancellationToken);

            if (legalState.IsLegal)
            {
                return ApplicationResultFactory.Success(new GetUserLegalStateQueryResult
                {
                    IsValid = true,
                    UserId = user.Id,
                });
            }

            var missings = new List<UserLegalRequirements>();
            if (legalState.TermsAndConditionsState != UserTermsAndConditionsState.TermsAccepted)
                missings.Add(UserLegalRequirements.TermsAndConditionsActualVersionAccepted);
            if (legalState.PrivacyPolicyState != UserPrivacyPolicyState.PrivacyPolicyAccepted)
                missings.Add(UserLegalRequirements.PrivacyPolicyActualVersionAccepted);

            return ApplicationResultFactory.Success(new GetUserLegalStateQueryResult
            {
                IsValid = false,
                UserId = user.Id,
                Missings = missings
            });
        }
    }
}
