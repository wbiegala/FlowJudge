using Microsoft.Extensions.Caching.Distributed;

namespace FlowJudge.Common.Cache.LayerCache
{
    internal sealed class SqlLayerCache(IDistributedCache sqlCache) : ILayerCache
    {
        public Task<byte[]?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            return sqlCache.GetAsync(key, cancellationToken);
        }

        public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            return sqlCache.RemoveAsync(key, cancellationToken);
        }

        public Task SetAsync(string key, byte[] value, TimeSpan? TTL, CancellationToken cancellationToken = default)
        {
            return sqlCache.SetAsync(key, value, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TTL
            }, cancellationToken);
        }
    }
}
