using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio_data_aggregate.Cache
{
    public class MemoryLanguageDataCache : ILanguageDataCache
    {
        private readonly IMemoryCache _cache;
        private const int _cacheExpirationInDays = 7;

        public MemoryLanguageDataCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        /// <summary>
        /// Obtains value from inmemory cache at key position
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>success status of get operation</returns>
        public bool TryGetValue(string key, out Dictionary<string, int> value)
        {
            return _cache.TryGetValue(key, out value);
        }

        /// <summary>
        /// Sets value in inmemory cache at key position
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>success status of set operation</returns>
        public bool TrySetValue(string key, Dictionary<string, int> value)
        {
            _cache.Set(key, value, TimeSpan.FromDays(_cacheExpirationInDays));
            return true;
        }
    }
}
