using DopamineDetoxAPI.Extensions;
using Microsoft.Extensions.Caching.Memory;

namespace DopamineDetoxAPI.Services
{
    public class CacheService
    {
        private readonly IMemoryCache _cache;

        public CacheService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public T GetOrCreate<T>(string cacheKey, Func<T> createItem, TimeSpan cacheDuration)
        {
            if (!_cache.TryGetValue(cacheKey, out T cacheEntry))
            {
                cacheEntry = createItem();
                var cacheEntryOptions = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = cacheDuration
                };
                _cache.Set(cacheKey, cacheEntry, cacheEntryOptions);
            }
            return cacheEntry;
        }

        public void Remove(string cacheKey)
        {
            _cache.Remove(cacheKey);
        }

        public void ClearAll()
        {
            var allKeys = _cache.GetKeys();
            foreach (var cacheKey in _cache.GetKeys())
            {
                _cache.Remove(cacheKey);
            }
        }
    }
}
