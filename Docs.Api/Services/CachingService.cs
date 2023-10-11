using Docs.Api.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace Docs.Api.Services
{
    public class CachingService<T> : ICachingService<T> where T : class
    {
        private readonly IMemoryCache _cache;

        public CachingService(IMemoryCache cache)
        {
            _cache = cache;
        }

        public void DeleteItems(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            _cache.Remove(key);
        }

        public T GetItem(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return _cache.Get<T>(key);
        }

        public List<T> GetItems(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            return _cache.Get<List<T>>(key);
        }

        public void SetItem(string key, T item)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (item == null)
                throw new ArgumentNullException(nameof(item));

            var memoryCacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10))
                                                                       .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(key, item, memoryCacheEntryOptions);
        }

        public void SetItems(string key, List<T> items)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (!items.Any())
                throw new ArgumentOutOfRangeException(nameof(items));

            var memoryCacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromMinutes(10))
                                                                       .SetAbsoluteExpiration(TimeSpan.FromMinutes(10));

            _cache.Set(key, items, memoryCacheEntryOptions);
        }
    }
}
