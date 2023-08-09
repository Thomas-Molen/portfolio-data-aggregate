using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio_data_aggregate.Cache
{
    public interface ILanguageDataCache
    {
        /// <summary>
        /// Obtains value from cache at key position
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>success status of get operation</returns>
        public bool TryGetValue(string key, out Dictionary<string, int> value);

        /// <summary>
        /// Sets value in cache at key position
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>success status of set operation</returns>
        public bool TrySetValue(string key, Dictionary<string, int> value);
    }
}
