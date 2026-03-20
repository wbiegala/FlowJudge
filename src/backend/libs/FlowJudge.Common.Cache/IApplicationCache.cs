using System;
using System.Collections.Generic;
using System.Text;

namespace FlowJudge.Common.Cache
{
    public interface IApplicationCache
    {
        Task SetAsync(string key, byte[] value, TimeSpan? TTL, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task<byte[]?> GetAsync(string key, CancellationToken cancellationToken = default);
    }
}
