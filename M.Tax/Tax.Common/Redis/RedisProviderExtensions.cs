using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tax.Common;
using Tax.Common.Extention;

namespace SissCloud.Caching.Redis
{
    public static class RedisProviderExtension
    {
        public const int Expires = 3600;//默认缓存时长1h
        /// <summary>
        /// 拼接Key前缀
        /// </summary>
        private static readonly string _cacheKeyPrefix = BaseCore.Configuration.GetValue<string>("WebsiteCacheKeyPrefix");

        public static bool Set(this RedisProvider rp, string key, string value, int expires = Expires)
        {
            return rp.DefaultDB.StringSet(key, value, TimeSpan.FromSeconds(expires));
        }
        public static string Get(this RedisProvider rp, string key)
        {
            return rp.DefaultDB.StringGet(key);
        }

        #region BulkRemove
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="key">通配key值，如【userId_*】->匹配以【userId_】开头的所有key</param>
        /// <param name="pageSize">页码数</param>
        public static long BulkRemove(this RedisProvider rp, string key, int pageSize = 5000)
        {
            var keys =rp.Conn.GetServer(rp.Conn.GetEndPoints()[0]).Keys(database: rp.DefaultDB.Database, pattern: key, pageSize: pageSize);
            return rp.DefaultDB.KeyDelete(keys.ToArray());
        }

        #endregion

        #region RemoveAll
        /// <summary>
        /// 清理整个db数据 
        /// </summary>
        public static void RemoveAll(this RedisProvider rp)
        {
            rp.DefaultDB.Execute(" flushdb ");
        }
        #endregion

        /// <summary>
        /// 设置哈希值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rp"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public static bool HashSet<T>(this RedisProvider rp, string key, T value, int expires = Expires)
        {
            var result = rp.DefaultDB.HashSet(key, "data", value.ToJson());
            if (expires > 0)
            {
                rp.KeyExpire(key, expires);
            }
            return result;
        }

        public static T HashGet<T>(this RedisProvider rp, string key)
        {
            var data = rp.DefaultDB.HashGet(key, "data");
            return data.ToString().ToObj<T>();
        }

        public static bool KeyExpire(this RedisProvider rp, string key, int expires = Expires)
        {
            return rp.DefaultDB.KeyExpire(key, TimeSpan.FromSeconds(expires));
        }

        public static bool Delete(this RedisProvider rp, string key, bool addPrefix)
        {
            if (addPrefix)
            {
                key = $"{_cacheKeyPrefix}:{key}";
            }

            return Delete(rp, key);
        }

        /// <summary>
        /// 删除指定key，默认不加前缀
        /// </summary>
        /// <param name="rp"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool Delete(this RedisProvider rp, string key)
        {
            return rp.DefaultDB.KeyDelete(key);
        }
        public static bool KeyExists(this RedisProvider rp, string key)
        {
            return rp.DefaultDB.KeyExists(key);
        }

        /// <summary>
        /// 根据包含关键词的key列表
        /// </summary>
        /// <param name="rp"></param>
        /// <param name="pattern">可包含通配符的key值，如【*userId_*】->匹配包含【userId_】的所有key</param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static List<string> GetKeyList(this RedisProvider rp, string pattern, int pageSize = 1000, bool addPrefix = true)
        {
            if (addPrefix && !pattern.StartsWith("*"))
            {
                pattern = $"{_cacheKeyPrefix}:{pattern}";
            }
            return rp.Conn.GetServer(rp.Conn.GetEndPoints()[0]).Keys(database: rp.DefaultDB.Database, pattern: pattern, pageSize: pageSize).ToList().ConvertAll(a => a.ToString());
        }
        /// <summary>
        /// 尝试从redis缓存中取数据，如果取不到，则将func的返回数据缓存到Cache中，并返回。
        /// 返回值为null不会缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rp"></param>
        /// <param name="key"></param>
        /// <param name="func"></param>
        /// <param name="expries">缓存时长（s）,-1为永不过期</param>
        /// <returns></returns>
        public static T TryToGetFromHashCache<T>(this RedisProvider rp, string key, Func<T> func, int expries = RedisProviderExtension.Expires)
        {
            if (rp.KeyExists(key))
            {
                return rp.HashGet<T>(key);
            }
            if (func == null)
            {
                return default(T);
            }
            var data = default(T);
            data = func();
            if (data != null)
            {
                rp.HashSet(key, data, expries);
            }

            return data;
        }

        /// <summary>
        ///获取redis事务锁
        /// </summary>
        /// <param name="rp"></param>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <param name="timeSpan"></param>
        /// <returns>获取锁是否成功</returns>
        public static bool LockTake(this RedisProvider rp, string key, string token, TimeSpan timeSpan)
        {
            return rp.DefaultDB.LockTake(key, token, timeSpan);
        }

        /// <summary>
        /// 释放redis事务锁
        /// </summary>
        /// <param name="rp"></param>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool LockRelease(this RedisProvider rp, string key, string token)
        {
            return rp.DefaultDB.LockRelease(key, token);
        }
        #region set集合数据类型的操作
        /// <summary>
        /// 获取整个set集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="addPrefix"></param>
        /// <returns></returns>
        public static List<T> SGet<T>(this RedisProvider rp, string key, bool addPrefix = true)
        {
            if (addPrefix)
            {
                key = $"{_cacheKeyPrefix}:{key}";
            }
            var data = rp.DefaultDB.SetMembers(key);
            return JsonConvert.DeserializeObject<List<T>>(data.ToJson());
        }

        public static bool SAdd<T>(this RedisProvider rp, string key, T value, bool addPrefix = true)
        {
            if (addPrefix)
            {
                key = $"{_cacheKeyPrefix}:{key}";
            }
            if (value.GetType() == typeof(string))
            {
                return rp.DefaultDB.SetAdd(key, value.ToString());
            }
            else
            {
                return rp.DefaultDB.SetAdd(key, value.ToJson());
            }
        }

        /// <summary>
        /// 向同一个set集合批量插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rp"></param>
        /// <param name="key"></param>
        /// <param name="valueList"></param>
        /// <param name="addPrefix"></param>
        public static void SBulkAdd<T>(this RedisProvider rp, string key, IEnumerable<T> valueList, bool addPrefix = true)
        {
            rp.SBulkTask(key, valueList, (batch, k, val) =>
            {
                return batch.SetAddAsync(k, val.ToJson());
            }, addPrefix);
        }

        /// <summary>
        /// 在同一个set集合中批量删除数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rp"></param>
        /// <param name="key"></param>
        /// <param name="valueList"></param>
        /// <param name="addPrefix"></param>
        public static void SBulkRemove<T>(this RedisProvider rp, string key, IEnumerable<T> valueList, bool addPrefix = true)
        {
            rp.SBulkTask(key, valueList, (batch, k, val) =>
            {
                return batch.SetRemoveAsync(k, val.ToJson());
            }, addPrefix);
        }

        /// <summary>
        ///set类型缓存，同一个key下数据的批量处理
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="rp"></param>
        /// <param name="key"></param>
        /// <param name="valueList"></param>
        /// <param name="taskFunc"></param>
        /// <param name="addPrefix"></param>
        private static void SBulkTask<T>(this RedisProvider rp, string key, IEnumerable<T> valueList, Func<IBatch, string, string, Task> taskFunc, bool addPrefix)
        {
            if (taskFunc == null)
            {
                return;
            }
            if (addPrefix)
            {
                key = $"{_cacheKeyPrefix}:{key}";
            }
            var batch = rp.DefaultDB.CreateBatch();
            var tasks = new List<Task>();

            foreach (var val in valueList)
            {
                tasks.Add(taskFunc.Invoke(batch, key, val.ToJson()));
            }

            batch.Execute();
            var result = rp.DefaultDB.SetMembersAsync(key);
            tasks.Add(result);
            Task.WhenAll(tasks.ToArray());
        }
        /// <summary>
        /// 判断key下的set中是否包含指定值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="addPrefix"></param>
        /// <returns></returns>
        public static bool SContains<T>(this RedisProvider rp, string key, T value, bool addPrefix = true)
        {
            if (addPrefix)
            {
                key = $"{_cacheKeyPrefix}:{key}";
            }

            return rp.DefaultDB.SetContains(key, value.ToJson());
        }
        /// <summary>
        /// 从指定集合中移除itemValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="addPrefix"></param>
        /// <returns></returns>
        public static bool SRemove<T>(this RedisProvider rp, string key, T value, bool addPrefix = true)
        {
            if (addPrefix)
            {
                key = $"{_cacheKeyPrefix}:{key}";
            }
            return rp.DefaultDB.SetRemove(key, value.ToJson());
        }

        public static T SPop<T>(this RedisProvider rp, string key, bool addPrefix = true)
        {
            if (addPrefix)
            {
                key = $"{_cacheKeyPrefix}:{key}";
            }

            var data = rp.DefaultDB.SetPop(key);
            return data.ToString().ToObj<T>();
        }
        #endregion set集合数据类型的操作
    }
}
