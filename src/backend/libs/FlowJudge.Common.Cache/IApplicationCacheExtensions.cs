namespace FlowJudge.Common.Cache
{
    public static class IApplicationCacheExtensions
    {
        public static async Task SetStringAsync(this IApplicationCache cache,
            string key,
            string value,
            TimeSpan? TTL,
            CancellationToken cancellationToken = default)
        {
            var bytes = System.Text.Encoding.UTF8.GetBytes(value);

            await cache.SetAsync(key, bytes, TTL, cancellationToken);
        }

        public static async Task SetObjectAsync<TObject>(
            this IApplicationCache cache,
            string key,
            TObject obj,
            TimeSpan? TTL,
            CancellationToken cancellationToken = default)
        {
            var bytes = System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj);

            await cache.SetAsync(key, bytes, TTL, cancellationToken);
        }

        public static async Task<string?> GetStringAsync(this IApplicationCache cache,
            string key, 
            CancellationToken cancellationToken = default)
        {
            var bytes = await cache.GetAsync(key, cancellationToken);
            if (bytes is null)
                return null;
            return System.Text.Encoding.UTF8.GetString(bytes);
        }

        public static async Task<TObject?> GetObjectAsync<TObject>(
            this IApplicationCache cache,
            string key, 
            CancellationToken cancellationToken = default)
        {
            var bytes = await cache.GetAsync(key, cancellationToken);
            if (bytes is null)
                return default;

            return System.Text.Json.JsonSerializer.Deserialize<TObject>(bytes);
        }
    }
}
