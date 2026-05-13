using FlowJudge.Common.Application;
using FlowJudge.Common.Application.Mediator;
using FlowJudge.Users.Application.Abstractions.Ports;
using FlowJudge.Users.Application.Abstractions.Queries;
using FlowJudge.Users.Application.Models;
using FlowJudge.Users.Domain.Model;

namespace FlowJudge.Users.Application.Queries
{
    internal sealed class GetUserDataQueryHandler : IQueryHandler<GetUserDataQuery, UserData>
    {
        private readonly IUserRepository _userRepository;

        public GetUserDataQueryHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IResult<UserData>> HandleAsync(
            GetUserDataQuery query,
            CancellationToken cancellationToken = default)
        {
            return await _userRepository.GetUserByIdentityIdAsync(query.UserId, cancellationToken) is User user
                ? ApplicationResultFactory.Success(new UserData { UserId = user.IdentityId, UserName = user.UserName, EmailAddress = user.EmailAddress })
                : ApplicationResultFactory.Failure<UserData>("User not found", ErrorCodeGenerator.NotFound(nameof(user)));
        }
    }
}
