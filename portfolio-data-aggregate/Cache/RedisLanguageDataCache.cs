using Microsoft.Extensions.Logging;
using portfolio_data_aggregate.Cache;
using portfolio_data_aggregate.Utility;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio_data_aggregate.Cache
{
    public class RedisLanguageDataCache : ILanguageDataCache
    {
        private readonly IRedisCacheConnector _cacheConnector;
        private readonly string _cacheConnectionString;

        public RedisLanguageDataCache(IRedisCacheConnector cacheConnector)
        {
            _cacheConnector = cacheConnector;
            _cacheConnectionString = Environment.GetEnvironmentVariable("RedisCacheConnectionString");
        }

        /// <summary>
        /// Attempts to obtain value from cache at key position
        /// </summary>
        /// <param name="key"></param>
        /// <param name="result"></param>
        /// <returns>if data was obtained successfully</returns>
        public bool TryGetValue(string key, out Dictionary<string, int> result)
        {
            IDatabase dbCache = _cacheConnector.getDbCache(_cacheConnectionString);
            string stringResult = (string)dbCache.StringGet(key);
            result = DictionaryCacheFactory.ToDictionary(stringResult);
            return result is not null;
        }

        /// <summary>
        /// Attempts to set value in cache at key position
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>if set was successfull</returns>
        public bool TrySetValue(string key, Dictionary<string, int> value)
        {
            IDatabase dbCache = _cacheConnector.getDbCache(_cacheConnectionString);
            return dbCache.StringSet(key, DictionaryCacheFactory.ToString(value), TimeSpan.FromDays(7));
        }
    }
}
