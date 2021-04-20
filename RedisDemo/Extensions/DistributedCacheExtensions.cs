using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace RedisDemo.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache,string recordId,T data,TimeSpan? absoulteExpireTime =null,TimeSpan? unusedExpireTime=null )
        {
            var options = new DistributedCacheEntryOptions();
            options.AbsoluteExpirationRelativeToNow = absoulteExpireTime ?? TimeSpan.FromSeconds(60);
            options.SlidingExpiration = unusedExpireTime;
            var jsondata = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordId, jsondata, options);
        }

        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache,string recordId)
        {
            var jsondata = await cache.GetStringAsync(recordId);
            if(jsondata is null)
            {
                return default(T);
            }
            return JsonSerializer.Deserialize<T>(jsondata);
        }
    }
}
