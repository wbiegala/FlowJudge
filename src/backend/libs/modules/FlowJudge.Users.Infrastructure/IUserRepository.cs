using FlowJudge.Users.Domain.Model;

namespace FlowJudge.Users.Infrastructure
{
    public interface IUserRepository
    {
        Task<User?> GetUserByIdentityIdAsync(Guid identityId, CancellationToken cancellationToken = default);
        Task AddUserAsync(User user, CancellationToken cancellationToken = default);
        Task UpdateUserAsync(User user, CancellationToken cancellationToken = default);
    }
}
