namespace FlowJudge.API.Contracts.User
{
    public sealed record GetUserLegalStateResponse
    {
        public bool IsLegal { get; init; }
        public IEnumerable<string>? Missings { get; init; }
    }
}
