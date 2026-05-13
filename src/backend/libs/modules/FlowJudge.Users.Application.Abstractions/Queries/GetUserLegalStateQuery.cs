using FlowJudge.Common.Application.Mediator;

namespace FlowJudge.Users.Application.Abstractions.Queries
{
    public sealed record GetUserLegalStateQuery(Guid UserIdentityId) : IQuery<GetUserLegalStateQueryResult>;
}
