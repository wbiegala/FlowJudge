using FlowJudge.Common.Application.Mediator;

namespace FlowJudge.Users.Application.Queries
{
    public sealed record GetUserLegalStateQuery(Guid UserIdentityId) : IQuery<GetUserLegalStateQueryResult>;
}
