namespace FlowJudge.API.Contracts.Auth
{
    public sealed record GetUserDataResponse
    {
        public Guid Id { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
    }
}
