using FlowJudge.Common.Application.Mediator;
using FlowJudge.Users.Application.Models;

namespace FlowJudge.Users.Application.Abstractions.Queries
{
    public sealed record GetUserDataQuery(Guid UserId) : IQuery<UserData>;
}
