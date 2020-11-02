using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using Tax.Common;

namespace SissCloud.Caching.Redis
{
    public class RedisProvider
    {
        public RedisProvider(RedisType redisType)
        {
            Conn = ConnectionMultiplexer.Connect(BaseCore.AppSetting.GetSection("RedisConfig").GetValue<string>(redisType.ToString()));
            RedisType = redisType;
        }

        internal ConnectionMultiplexer Conn { get; }

        internal IDatabase DefaultDB
        {
            get { return Conn.GetDatabase(); }
        }
        public RedisType RedisType { get; }
    }
}
