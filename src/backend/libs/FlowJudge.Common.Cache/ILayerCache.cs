namespace FlowJudge.Common.Cache
{
    internal interface ILayerCache
    {
        Task SetAsync(string key, byte[] value, TimeSpan? TTL, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task<byte[]?> GetAsync(string key, CancellationToken cancellationToken = default);
    }
}
