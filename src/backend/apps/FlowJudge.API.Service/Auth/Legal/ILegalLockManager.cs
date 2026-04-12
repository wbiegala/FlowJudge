using FlowJudge.Users.Application.Models;

namespace FlowJudge.API.Service.Auth.Legal
{
    public interface ILegalLockManager
    {
        Task<bool> CheckLegalAsync(Guid userIdentityId, CancellationToken cancellationToken = default);
        Task AddLegalLockAsync(Guid userIdentityId, IEnumerable<UserLegalRequirements> missings, CancellationToken cancellationToken = default);
        Task ReleaseLockAsync(Guid userIdentityId, IEnumerable<UserLegalRequirements> missings, CancellationToken cancellationToken = default);
    }

    public static class ILegalLockManagerExtensions
    {
        public static async Task ReleaseLockAsync(
            this ILegalLockManager manager,
            Guid userIdentityId,
            UserLegalRequirements missing,
            CancellationToken cancellationToken = default) =>
                await manager.ReleaseLockAsync(userIdentityId, new List<UserLegalRequirements> { missing }, cancellationToken);
    }
}
