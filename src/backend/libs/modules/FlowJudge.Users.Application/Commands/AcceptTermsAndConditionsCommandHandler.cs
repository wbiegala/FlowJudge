using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Transactional;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Common.Utils.Time;
using FlowJudge.Users.Application.Abstractions.Commands;
using FlowJudge.Users.Application.Abstractions.Ports;
using FlowJudge.Users.Application.Extensions;

namespace FlowJudge.Users.Application.Commands
{
    internal sealed class AcceptTermsAndConditionsCommandHandler : TransactionalCommandHandler<AcceptTermsAndConditionsCommand>
    {
        private readonly IUserRepository _userRepository;
        private readonly IDocumentVersionRepository _documentVersionRepository;
        private readonly ITimeService _timeService;

        public AcceptTermsAndConditionsCommandHandler(
            IUserRepository userRepository,
            IDocumentVersionRepository documentVersionRepository,
            ITimeService timeService,
            IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _userRepository = userRepository;
            _documentVersionRepository = documentVersionRepository;
            _timeService = timeService;
        }

        protected override async Task<IResult> ExecuteInTransactionAsync(AcceptTermsAndConditionsCommand command, CancellationToken cancellationToken = default)
        {
            var user = await _userRepository.GetUserByIdentityIdAsync(command.UserIdentityId, cancellationToken);

            if (user is null)
            {
                return ApplicationResultFactory.Failure($"User with given identityId={command.UserIdentityId} not found.",
                    ErrorCodeGenerator.NotFound(nameof(user)));
            }

            var termsAndConditionsVersion = await _documentVersionRepository.GetTermsAndConditionsByIdAsync(command.TermsAndConditionsVersionId, cancellationToken);

            if (termsAndConditionsVersion is null)
            {
                return ApplicationResultFactory.Failure($"Terms and conditions version with given id={command.TermsAndConditionsVersionId} not found.",
                    ErrorCodeGenerator.NotFound("terms_and_conditions_version"));
            }

            if (!termsAndConditionsVersion.IsAcceptable)
            {
                return ApplicationResultFactory.Failure($"Terms and conditions version with given id={command.TermsAndConditionsVersionId} is not acceptable.",
                    ErrorCodeGenerator.NotAcceptable("terms_and_conditions_version"));
            }

            user.AcceptTermsAndConditions(termsAndConditionsVersion.Number, _timeService.UtcNow);

            await _userRepository.UpdateUserAsync(user, cancellationToken);

            return ApplicationResultFactory.Success();
        }
    }
}
