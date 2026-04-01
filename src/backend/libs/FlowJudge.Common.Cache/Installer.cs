using FlowJudge.Common.Cache.LayerCache;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Postgres;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Timeout;

namespace FlowJudge.Common.Cache
{
    public static class Installer
    {
        public static IServiceCollection AddCache(
            this IServiceCollection services,
            Action<CacheConfiguration> configure)
        {
            var configuration = new CacheConfiguration();
            configure(configuration);

            services.AddRedisResiliencePolicy(configuration);

            services.AddSingleton(configuration);

            services.AddKeyedSingleton<IDistributedCache>("sql", (sp, key) =>
            {
                var cfg = sp.GetRequiredService<CacheConfiguration>();
                var options = new PostgresCacheOptions
                {
                    ConnectionString = cfg.SqlConnectionString,
                    SchemaName = cfg.SqlSchemaName,
                    TableName = cfg.SqlTableName,
                    CreateIfNotExists = true,
                };

                return new PostgresCache(options);
            });

            services.AddKeyedSingleton<IDistributedCache>("redis", (sp, key) =>
            {
                var cfg = sp.GetRequiredService<CacheConfiguration>();
                var options = new RedisCacheOptions
                {
                    Configuration = cfg.RedisConnectionString,
                    InstanceName = cfg.CacheName
                };
                return new RedisCache(options);
            });

            services.AddSingleton(sp =>
            {
                var sqlCache = sp.GetRequiredKeyedService<IDistributedCache>("sql");

                return new SqlLayerCache(sqlCache);
            });

            services.AddSingleton(sp =>
            {
                var redisCache = sp.GetRequiredKeyedService<IDistributedCache>("redis");

                return new RedisLayerCache(redisCache);
            });

            services.AddSingleton<IApplicationCache>(sp =>
            {
                var resiliencePolicyProvider = sp.GetRequiredService<ResiliencePipelineProvider<string>>();

                return new ApplicationCacheOrchestrator(
                    sp.GetRequiredService<SqlLayerCache>(),
                    sp.GetRequiredService<RedisLayerCache>(),
                    resiliencePolicyProvider.GetPipeline(CacheConfiguration.ReadRedisResiliencePolicyName),
                    resiliencePolicyProvider.GetPipeline(CacheConfiguration.WriteRedisResiliencePolicyName),
                    sp.GetRequiredService<ILogger<ApplicationCacheOrchestrator>>());
            });

            return services;
        }

        private static IServiceCollection AddRedisResiliencePolicy(this IServiceCollection services, CacheConfiguration configuration)
        {
            services.AddResiliencePipeline(CacheConfiguration.ReadRedisResiliencePolicyName, builder =>
            {
                builder.AddTimeout(TimeSpan.FromMilliseconds(configuration.ReadRedisResilienceOptions.TimeoutInMilliseconds));

                builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder()
                        .Handle<TimeoutRejectedException>()
                        .Handle<Exception>(ex =>
                            ex.GetType().Name is "RedisTimeoutException" or "RedisConnectionException"
                                || ex.InnerException?.GetType().Name is "RedisTimeoutException" or "RedisConnectionException"),
                    FailureRatio = configuration.ReadRedisResilienceOptions.FailureRatio,
                    SamplingDuration = TimeSpan.FromSeconds(configuration.ReadRedisResilienceOptions.SamplingDurationInSeconds),
                    MinimumThroughput = configuration.ReadRedisResilienceOptions.MinimumThroughput,
                    BreakDuration = TimeSpan.FromSeconds(configuration.ReadRedisResilienceOptions.BreakDurationInSeconds)
                });
            });

            services.AddResiliencePipeline(CacheConfiguration.WriteRedisResiliencePolicyName, builder =>
            {
                builder.AddTimeout(TimeSpan.FromMilliseconds(configuration.WriteRedisResilienceOptions.TimeoutInMilliseconds));

                builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions
                {
                    ShouldHandle = new PredicateBuilder()
                        .Handle<TimeoutRejectedException>()
                        .Handle<Exception>(ex =>
                            ex.GetType().Name is "RedisTimeoutException" or "RedisConnectionException"
                                || ex.InnerException?.GetType().Name is "RedisTimeoutException" or "RedisConnectionException"),
                    FailureRatio = configuration.WriteRedisResilienceOptions.FailureRatio,
                    SamplingDuration = TimeSpan.FromSeconds(configuration.WriteRedisResilienceOptions.SamplingDurationInSeconds),
                    MinimumThroughput = configuration.WriteRedisResilienceOptions.MinimumThroughput,
                    BreakDuration = TimeSpan.FromSeconds(configuration.WriteRedisResilienceOptions.BreakDurationInSeconds)
                });
            });

            return services;
        }
    }
}
