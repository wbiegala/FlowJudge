using FlowJudge.Common.Application.Mediator;

namespace FlowJudge.Users.Application.Abstractions.Commands
{
    public sealed record SynchronizeUserCommand : ICommand
    {
        public required string UserId { get; init; }
        public required string Username { get; init; }
        public required string Email { get; init; }
    }
}
