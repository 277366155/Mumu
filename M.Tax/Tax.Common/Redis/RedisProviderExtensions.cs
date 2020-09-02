using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SissCloud.Caching.Redis
{
    public static class RedisProviderExtensions
    {
        /// <summary>
        ///尝试从redis中取数据，如果不存在，则通过取func值，并缓存至redis
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="redisProvider"></param>
        /// <param name="key"></param>
        /// <param name="cacheTime">缓存时长（分钟） 值为null时使用默认时间7天,-1为永不过期</param>
        /// <param name="lockKey">公共事务锁key</param>
        /// <param name="func"></param>
        /// <param name="lockExpire">事务锁过期时间（s）</param>
        /// <param name="nullCacheTime">缓存数据为null时的缓存时常（分钟），默认null取cacheTime</param>
        /// <returns></returns>
        public static T TryToGetDataFromCache<T>(this RedisProvider redisProvider, string key,int? cacheTime, string lockKey, Func<T> func, double lockExpire = 30,int?  nullCacheTime=null)
        {
            if (redisProvider.HasKey(key))
            {
                return redisProvider.Get<T>(key);
            }
            if (func == null)
            {
                return default(T);
            }

            var data = default(T);
            
            Action setAct = ()=>{
                data = func();
                //如果设置了空值缓存时间，并且data为空值。
                if (nullCacheTime.HasValue && data == null)
                {
                    redisProvider.Set(key, data, nullCacheTime.Value);
                }
                //未设置空值缓存时间或者data不为null
                else 
                {
                    if (cacheTime.HasValue)
                        redisProvider.Set(key, data, cacheTime.Value);
                    else
                        redisProvider.Set(key, data);
                }
            };
            if (string.IsNullOrWhiteSpace(lockKey))
            {
                setAct();
            }
            else
            {
                if (key == lockKey)
                {
                    throw new ArgumentException("数据key和事务锁的key值不能相同");
                }
                using (var rLock=redisProvider.GetLock(lockKey, lockExpire))
                {
                    if ((rLock as RedisLockTake).LockResult)
                    {
                        //双检机制，防止在上面判断之后在下面set之前被其他线程写入
                        if (redisProvider.HasKey(key))
                        {
                            return redisProvider.Get<T>(key);
                        }
                        setAct();
                    }
                }
            }
            return data;
        }
    }
}
