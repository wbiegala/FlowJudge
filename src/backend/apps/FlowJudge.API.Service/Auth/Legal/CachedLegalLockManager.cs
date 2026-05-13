using FlowJudge.Common.Application.Mediator;
using FlowJudge.Common.Cache;
using FlowJudge.Users.Application.Abstractions.Queries;
using FlowJudge.Users.Application.Models;
using System.Text;
using System.Text.Json;

namespace FlowJudge.API.Service.Auth.Legal
{
    internal sealed class CachedLegalLockManager : ILegalLockManager
    {
        private readonly IMediator _mediator;
        private readonly IApplicationCache _cache;

        public CachedLegalLockManager(IMediator mediator, IApplicationCache cache)
        {
            _mediator = mediator;
            _cache = cache;
        }

        public async Task AddLegalLockAsync(Guid userIdentityId, IEnumerable<UserLegalRequirements> missings, CancellationToken cancellationToken = default)
        {
            await SetLockAsync(userIdentityId, missings, cancellationToken);
        }

        public async Task<bool> CheckLegalAsync(Guid userIdentityId, CancellationToken cancellationToken = default)
        {
            var currentLock = await GetLockAsync(userIdentityId, cancellationToken);
            if (currentLock is null)
            {
                var hasLegalMarker = await IsLegalMarkerAsync(userIdentityId, cancellationToken);
                if (hasLegalMarker)
                {
                    await SetLegalMarkerAsync(userIdentityId, cancellationToken);

                    return true;
                }

                var query = new GetUserLegalStateQuery(userIdentityId);
                var result = await _mediator.SendQueryAsync<GetUserLegalStateQuery, GetUserLegalStateQueryResult>(query, cancellationToken);
                if (result is null || !result.IsSuccess) throw new InvalidOperationException();

                if (result.Data!.IsValid)
                {
                    await SetLegalMarkerAsync(userIdentityId, cancellationToken);

                    return true;
                }

                await SetLockAsync(userIdentityId, result.Data.Missings!, cancellationToken);
                return false;
            }

            await SetLockAsync(userIdentityId, currentLock, cancellationToken);

            return false;
        }

        public async Task ReleaseLockAsync(Guid userIdentityId, IEnumerable<UserLegalRequirements> missingsToRelese, CancellationToken cancellationToken = default)
        {
            var currentLock = await GetLockAsync(userIdentityId, cancellationToken);
            if (currentLock is null || !currentLock.Any())
                return;

            var newLock = currentLock.ToList();

            foreach (var missingToRelease in missingsToRelese)
            {
                newLock.Remove(missingToRelease);
            }

            if (newLock.Any())
            {
                await SetLockAsync(userIdentityId, newLock, cancellationToken);
            }
            else
            {
                await SetLegalMarkerAsync(userIdentityId, cancellationToken);
                await _cache.RemoveAsync(GetLockKey(userIdentityId), cancellationToken);
            }
        }

        private async Task<IEnumerable<UserLegalRequirements>?> GetLockAsync(Guid userIdentityId, CancellationToken cancellationToken = default)
        {
            var serializedMissingsBytes = await _cache.GetAsync(GetLockKey(userIdentityId), cancellationToken);
            if (serializedMissingsBytes is null)
                return null;

            var serializedMissingsJson = Encoding.UTF8.GetString(serializedMissingsBytes);
            var missingsString = JsonSerializer.Deserialize<IEnumerable<string>>(serializedMissingsJson);

            return missingsString?.Select(x => Enum.Parse<UserLegalRequirements>(x));
        }

        private async Task SetLockAsync(Guid userIdentityId, IEnumerable<UserLegalRequirements> missings, CancellationToken cancellationToken = default)
        {
            await RemoveLegalMarkerAsync(userIdentityId, cancellationToken);

            var key = GetLockKey(userIdentityId);
            var existingLock = await _cache.GetAsync(key, cancellationToken);
            if (existingLock is not null)
            {
                await _cache.RemoveAsync(key, cancellationToken);
            }

            var missingsString = missings.Select(x => x.ToString());
            var serializedMissingsJson = JsonSerializer.Serialize(missingsString);
            var serializedMissingsBytes = Encoding.UTF8.GetBytes(serializedMissingsJson);

            await _cache.SetAsync(key, serializedMissingsBytes, TimeSpan.FromMinutes(LockTimeToLiveInMinutes), cancellationToken);
        }

        private async Task<bool> IsLegalMarkerAsync(Guid userIdentityId, CancellationToken cancellationToken = default)
        {
            var marker = await _cache.GetAsync(GetLegalKey(userIdentityId), cancellationToken);

            return marker is not null;
        }

        private async Task SetLegalMarkerAsync(Guid userIdentityId, CancellationToken cancellationToken = default)
        {
            var isLegalSet = await _cache.GetAsync(GetLegalKey(userIdentityId), cancellationToken);
            if (isLegalSet is not null)
            {
                await _cache.RemoveAsync(GetLegalKey(userIdentityId), cancellationToken);
            }
            await _cache.SetAsync(GetLegalKey(userIdentityId), OkContent, TimeSpan.FromMinutes(LockTimeToLiveInMinutes), cancellationToken);
        }

        private async Task RemoveLegalMarkerAsync(Guid userIdentityId, CancellationToken cancellationToken = default)
        {
            var isLegalSet = await _cache.GetAsync(GetLegalKey(userIdentityId), cancellationToken);
            if (isLegalSet is not null)
            {
                await _cache.RemoveAsync(GetLegalKey(userIdentityId), cancellationToken);
            }
        }

        private static string GetLockKey(Guid userIdentityId) =>
            $":legal-lock:{userIdentityId}";

        private static string GetLegalKey(Guid userIdentityId) =>
            $":legal-ok:{userIdentityId}";

        private static byte[] OkContent => [ 0 ];

        private const int LockTimeToLiveInMinutes = 60;
    }
}
