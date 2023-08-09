using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace portfolio_data_aggregate.Cache
{
    public interface IRedisCacheConnector
    {
        /// <summary>
        /// Return the redis cache database based on connection string
        /// </summary>
        /// <param name="cacheConnectionString">redis cache connection string</param>
        /// <returns></returns>
        IDatabase getDbCache(string cacheConnectionString);
    }
}
