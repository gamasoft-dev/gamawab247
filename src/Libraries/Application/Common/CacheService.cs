using Application.DTOs;
using Application.Services.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly string cacheKeyPrefix = "";
        private readonly RedisCacheConfig config;
        private readonly ILogger<CacheService> _logger;


        public CacheService(IDistributedCache cache, IOptions<RedisCacheConfig> options, ILogger<CacheService> logger)
        {
            _logger = logger;
            _cache = cache;
            config = options.Value;
            cacheKeyPrefix = config?.Env ?? "prod"; //optional: In the case we choose to use and differentiate base-off env
        }

        public async Task AddToCacheAsync<T>(string key, T payload)
        {
            try
            {
                var stringifiedJson = JsonConvert.SerializeObject(payload);

                //Set cache expiry time
                var cacheOptions = new DistributedCacheEntryOptions()
                                    .SetSlidingExpiration(TimeSpan.FromSeconds(config.CacheExpirationTime));

                await _cache.SetStringAsync(key, stringifiedJson, cacheOptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                throw;
            }
        }


        public async Task<T> ReadFromCacheAsync<T>(string key) where T : class
        {
            try
            {
                string serialisedPayload = await _cache.GetStringAsync(key);

                if (!string.IsNullOrWhiteSpace(serialisedPayload))
                {
                    T payload = JsonConvert.DeserializeObject<T>(serialisedPayload);
                    return payload;
                }

                return default;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return default;
            }
        }

        public async Task RemoveFromCache(string key)
        {
            await _cache.RemoveAsync(key);
        }

        public async Task UpdateCache<T>(string key, T payload) where T : class
        {
            await _cache.RemoveAsync(key);

            await AddToCacheAsync(key, payload);

        }
    }
}
