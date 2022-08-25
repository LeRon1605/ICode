using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;
using System;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Logging;
using Microsoft.Extensions.Logging;

namespace API.Extension
{
    public static class IDistributedCacheExtension
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache, string recordId, T data, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
        {
            // configuration for item which will be set in cache

            // convert data to json string
            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordId, jsonData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60),
                // expire data if dont use within unusedexpiretime
                SlidingExpiration = unusedExpireTime
            });

            ILogger<IDistributedCache> logger = LoggerFactory.Create(config =>
            {
                config.AddConsole();
                config.AddDebug();
            }).CreateLogger<IDistributedCache>();
            logger.LogInformation("Caching data to redis, RecordID: {RecordID}, ExpireAt: {ExpireAt}", recordId, DateTime.Now.Add(absoluteExpireTime ?? TimeSpan.FromSeconds(60)));
        }

        public static async Task<T> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetAsync(recordId);
            if (jsonData is null)
            {
                return default(T);
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
