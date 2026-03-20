using Microsoft.Extensions.Caching.Distributed;

namespace FlowJudge.Common.Cache.LayerCache
{
    internal sealed class RedisLayerCache(IDistributedCache redisCache) : ILayerCache
    {
        public Task<byte[]?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            return redisCache.GetAsync(key, cancellationToken);
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return redisCache.RemoveAsync(key, cancellationToken);
        }

        public Task SetAsync(string key, byte[] value, TimeSpan? TTL, CancellationToken cancellationToken = default)
        {
            return redisCache.SetAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TTL
            }, cancellationToken);
        }
    }
}
