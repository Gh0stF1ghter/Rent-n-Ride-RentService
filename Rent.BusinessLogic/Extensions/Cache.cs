using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Rent.BusinessLogic.Extensions;

internal static class Cache
{
    public static async Task<T?> GetDataFromCacheAsync<T>(this IDistributedCache distributedCache, string key, CancellationToken cancellationToken)
    {
        var cache = await distributedCache.GetStringAsync(key, cancellationToken);

        if (cache is null)
            return default;

        var data = JsonConvert.DeserializeObject<T>(cache)!;

        return data;
    }

    public static async Task CacheData<T>(this IDistributedCache distributedCache, T data, TimeSpan cacheLifetime, string key, CancellationToken cancellationToken)
    {
        if (data is null)
            return;

        var serializedData = JsonConvert.SerializeObject(data);

        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = cacheLifetime,
            SlidingExpiration = TimeSpan.FromMinutes(2)
        };

        await distributedCache.SetStringAsync(key, serializedData, options, cancellationToken);
    }
}