using FlowJudge.Users.Domain.Model;

namespace FlowJudge.Users.UnitTests.Application
{
    internal static class Fixture
    {
        public static User CreateUser(Guid id, Guid identityId, string username, string email) =>
            User.Load(id, identityId, username, email);
    }
}
