using System.Runtime.CompilerServices;

namespace SissCloud.Caching.Redis
{
    public class RedisHelper
    {
        static RedisClient client;
        static RedisHelper()
        {
            client = new RedisClient();
        }
        public static RedisProvider CacheMaster => client.GetRedisProvider(RedisType.CACHE_MASTER);
        public static RedisProvider CacheBusiness => client.GetRedisProvider(RedisType.CACHE_BUSINESS);
        public static RedisProvider CacheAccount => client.GetRedisProvider(RedisType.CACHE_SESSION);

    }
}
