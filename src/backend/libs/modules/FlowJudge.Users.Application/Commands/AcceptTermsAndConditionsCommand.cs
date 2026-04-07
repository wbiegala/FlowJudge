using FlowJudge.Common.Application.Mediator;

namespace FlowJudge.Users.Application.Commands
{
    public sealed record AcceptTermsAndConditionsCommand(Guid UserIdentityId, Guid TermsAndConditionsVersionId)
        : ICommand;
}
