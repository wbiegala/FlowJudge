using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Transactional;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Common.Utils.Time;
using FlowJudge.Users.Application.Abstractions;
using FlowJudge.Users.Application.Abstractions.Commands;
using FlowJudge.Users.Application.Abstractions.Ports;
using FlowJudge.Users.Application.Extensions;

namespace FlowJudge.Users.Application.Commands
{
    internal sealed class AcceptPrivacyPolicyCommandHandler : TransactionalCommandHandler<AcceptPrivacyPolicyCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IDocumentVersionRepository _documentVersionRepository;
        private readonly ITimeService _timeService;

        public AcceptPrivacyPolicyCommandHandler(
            IUserRepository userRepository,
            IDocumentVersionRepository documentVersionRepository,
            ITimeService timeService,
            IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
            _userRepository = userRepository;
            _documentVersionRepository = documentVersionRepository;
            _timeService = timeService;
        }

        protected override async Task<IResult> ExecuteInTransactionAsync(AcceptPrivacyPolicyCommand command, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByIdentityIdAsync(command.UserIdentityId, cancellationToken);

            if (user is null)
            {
                return ApplicationResultFactory.Failure($"User with given identityId={command.UserIdentityId} not found.", ErrorCodes.UserNotFound);
            }

            var privacyPolicyVersion = await _documentVersionRepository.GetPrivacyPolicyByIdAsync(command.PrivacyPolicyVersionId, cancellationToken);

            if (privacyPolicyVersion is null)
            {
                return ApplicationResultFactory.Failure($"Privacy policy version with given id={command.PrivacyPolicyVersionId} not found.",
                    ErrorCodes.PrivacyPolicyVersionNotFound);
            }

            if (!privacyPolicyVersion.IsAcceptable)
            {
                return ApplicationResultFactory.Failure($"Privacy policy version with given id={command.PrivacyPolicyVersionId} is not acceptable.",
                    ErrorCodes.PrivacyPolicyVersionNotFound);
            }

            user.AcceptPrivacyPolicy(privacyPolicyVersion.Number, _timeService.UtcNow);

            await _userRepository.UpdateUserAsync(user, cancellationToken);

            return ApplicationResultFactory.Success();
        }
    }
}
