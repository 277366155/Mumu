using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SissCloud.Caching.Redis
{
    public class RedisClient
    {
        private static ConcurrentDictionary<RedisType, RedisProvider> _connections;
        static Object lockObj = new object();
        public RedisClient()
        {
            _connections = new ConcurrentDictionary<RedisType, RedisProvider>();
        }

        public RedisProvider GetRedisProvider(RedisType redisType = RedisType.CACHE_MASTER)
        {
            if (!_connections.ContainsKey(redisType))
            {
                lock (lockObj)
                {
                    if (!_connections.ContainsKey(redisType))
                    {
                        var provider = new RedisProvider(redisType);
                        _connections.TryAdd(redisType, provider);
                        return provider;
                    }
                }
            }
            RedisProvider result;
            if (_connections.TryGetValue(redisType, out result))
                return result;
            throw new System.Exception("Error : RedisProvider Is Null ");
        }
    }
}