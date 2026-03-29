namespace FlowJudge.Common.Http.User
{
    public sealed record UserContext
    {
        public Guid Id { get; init; }
        public string Name { get; init; }
        public string Username { get; init; }
        public string Email { get; init; }
        public string Locale { get; init; }
        public IEnumerable<RealmRole> RealmRoles { get; init; }
    }
}
