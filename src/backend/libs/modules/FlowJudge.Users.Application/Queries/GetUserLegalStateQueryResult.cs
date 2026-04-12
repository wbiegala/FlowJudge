using FlowJudge.Users.Application.Models;

namespace FlowJudge.Users.Application.Queries
{
    public sealed record GetUserLegalStateQueryResult
    {
        public Guid UserId { get; init; }
        public bool IsValid { get; init; }
        public IEnumerable<UserLegalRequirements>? Missings { get; init; }
    }
}
