using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;

namespace FlowJudge.Common.Cache
{
    internal sealed class ApplicationCacheOrchestrator(
        ILayerCache sqlCache,
        ILayerCache redisCache,
        ResiliencePipeline readPipeline,
        ResiliencePipeline writePipeline,
        ILogger<ApplicationCacheOrchestrator> logger)
            : IApplicationCache
    {
        public async Task<byte[]?> GetAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                var redisBytes = await readPipeline.ExecuteAsync(
                    async token => await redisCache.GetAsync(key, token), cancellationToken);

                if (redisBytes is not null)
                    return redisBytes;
            }
            catch (BrokenCircuitException ex)
            {
                logger.LogDebug(ex, "Redis circuit open for key={key}, falling back to SQL cache", key);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Redis read failed for key={key}, falling back to SQL cache", key);
            }

            var sqlBytes = await sqlCache.GetAsync(key, cancellationToken);

            if (sqlBytes is null)
                return null;

            await TryWarmRedisAsync(key, sqlBytes, TimeSpan.FromMinutes(15), cancellationToken);

            return sqlBytes;
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await sqlCache.RemoveAsync(key, cancellationToken);

            try
            {
                await writePipeline.ExecuteAsync(
                    async token =>
                    {
                        await redisCache.RemoveAsync(key, token);
                    }, cancellationToken);

            }
            catch (BrokenCircuitException ex)
            {
                logger.LogDebug(ex, "Redis circuit open for key={key}, falling back to SQL cache", key);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Redis remove failed for key={key}, falling back to SQL cache", key);
            }
        }

        public async Task SetAsync(string key, byte[] value, TimeSpan? TTL, CancellationToken cancellationToken = default)
        {
            await sqlCache.SetAsync(key, value, TTL, cancellationToken);

            try
            {
                await writePipeline.ExecuteAsync(
                    async token =>
                    {
                        await redisCache.SetAsync(key, value, TTL, token);
                    }, cancellationToken);
            }
            catch (BrokenCircuitException ex)
            {
                logger.LogDebug(ex, "Redis circuit open for key={key}, falling back to SQL cache", key);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Redis write failed for key={key}, falling back to SQL cache", key);
            }
        }

        private async Task TryWarmRedisAsync(string key, byte[] value, TimeSpan ttl, CancellationToken ct)
        {
            try
            {
                await writePipeline.ExecuteAsync(
                    async token => await redisCache.SetAsync(key, value, ttl, token),
                    ct);
            }
            catch (BrokenCircuitException ex)
            {
                logger.LogDebug(ex, "Redis circuit open for key={key}, falling back to SQL cache", key);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Redis warm-up failed for key={key}, falling back to SQL cache", key);
            }
        }
    }
}
