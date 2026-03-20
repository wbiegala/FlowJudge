namespace FlowJudge.Common.Cache
{
    public sealed class CacheConfiguration
    {
        internal const string ReadRedisResiliencePolicyName = "redis-cache-read";
        internal const string WriteRedisResiliencePolicyName = "redis-cache-write";

        internal CacheConfiguration()
        {
            ReadRedisResilienceOptions = new RedisResilienceConfiguratio(150, 0.5, 30, 8, 60);
            WriteRedisResilienceOptions = new RedisResilienceConfiguratio(200, 0.5, 30, 5, 60);
        }

        internal string CacheName { get; private set; } = string.Empty;
        internal string RedisConnectionString { get; private set; } = string.Empty;
        internal string SqlConnectionString { get; private set; } = string.Empty;
        internal string SqlSchemaName { get; private set; } = string.Empty;
        internal string SqlTableName { get; private set; } = string.Empty;

        internal RedisResilienceConfiguratio ReadRedisResilienceOptions { get; private set; }
        internal RedisResilienceConfiguratio WriteRedisResilienceOptions { get; private set; }

        public void WithCacheName(string cacheName)
        {
            CacheName = cacheName;
        }

        public void UseRedis(string connectionString)
        {
            RedisConnectionString = connectionString;
        }

        public void UsePostgreSql(string connectionString, string schemaName = "dbo", string tableName = "ApplicationCache")
        {
            SqlConnectionString = connectionString;
            SqlSchemaName = schemaName;
            SqlTableName = tableName;
        }


        internal sealed record RedisResilienceConfiguratio(
            int TimeoutInMilliseconds,
            double FailureRatio,
            int SamplingDurationInSeconds,
            int MinimumThroughput,
            int BreakDurationInSeconds);
    }
}
