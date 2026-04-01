using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Transactional;
using FlowJudge.Common.Sql.UnitOfWork;
using FlowJudge.Shared.Domain.ValueObjects;
using FlowJudge.Users.Domain.Model;
using FlowJudge.Users.Infrastructure;

namespace FlowJudge.Users.Application.Commands
{
    internal sealed class SynchronizeUserCommandHandler : TransactionalCommandHandler<SynchronizeUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public SynchronizeUserCommandHandler(IUserRepository userRepository, IUnitOfWork unitOfWork) : base(unitOfWork)
        {
            _userRepository = userRepository;
        }

        protected override async Task<IResult> ExecuteInTransactionAsync(
            SynchronizeUserCommand command,
            CancellationToken cancellationToken = default)
        {
            var identityId = Guid.Parse(command.UserId);

            var user = await _userRepository.GetUserByIdentityIdAsync(identityId, cancellationToken);

            if (user is not null)
            {
                return ApplicationResultFactory.Success();
            }

            user = User.Create(
                UserIdentityId.Create(identityId),
                UserName.Create(command.Username),
                EmailAddress.Create(command.Email));

            await _userRepository.AddUserAsync(user, cancellationToken);

            return ApplicationResultFactory.Success();
        }
    }
}
