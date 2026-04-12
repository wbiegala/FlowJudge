using FlowJudge.Common.Application.Mediator;

namespace FlowJudge.Users.Application.Commands
{
    public sealed record AcceptPrivacyPolicyCommand(Guid UserIdentityId, Guid PrivacyPolicyVersionId) : ICommand;
}
