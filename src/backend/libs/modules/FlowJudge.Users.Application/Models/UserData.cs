namespace FlowJudge.Users.Application.Models
{
    public sealed record UserData
    {
        public Guid UserId { get; init; }
        public required string UserName { get; init; }
        public required string EmailAddress { get; init; }
    }
}
