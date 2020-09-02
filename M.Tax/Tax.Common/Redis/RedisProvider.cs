using Newtonsoft.Json;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Tax.Common;
using Tax.Common.Extention;

namespace SissCloud.Caching.Redis
{
    public class RedisProvider 
    {
        /// <summary>
        /// 默认过期时间（秒）
        /// </summary>
        public const int DefaultExpire = 10080 * 60;
        /// <summary>
        /// 当前过期时间
        /// </summary>
        int _expire;

        /// <summary>
        /// 拼接Key前缀
        /// </summary>
        private static readonly string _cacheKeyPrefix = BaseCore.AppSetting["CacheKeyPrefix"];

        public RedisProvider(RedisType redisType) : this(redisType, DefaultExpire)
        {

        }

        public RedisProvider(RedisType redisType, int expire = DefaultExpire)
        {
            try
            {
                RedisType = redisType;
                _expire = expire;
                InitRedis();
            }
            catch (Exception ex)
            {
                throw new Exception($"缓存{Hosts}异常：{ex.Message}");
            }
        }

        private void InitRedis()
        {
            var Hosts = BaseCore.AppSetting[RedisType.ToString()];
            if (!Hosts.IsNullOrWhiteSpace())
            {
                //格式为【127.0.0.1:6379,password=,defaultdatabase=2】
                Conn = ConnectionMultiplexer.Connect(Hosts);
            }
            else
            {
                throw new Exception($"缺少RedisCache配置项：{RedisType.ToString()}");
            }
        }

        #region Get
        public T Get<T>(string key)
        {
            return Get<T>(key, false,true);
        }

        /// <summary>
        /// 获取缓存中的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="extendCacheTime">是否延长缓存时间</param>
        /// <returns></returns>
        public T Get<T>(string key, bool extendCacheTime, bool addPrefix)
        {
            if (addPrefix)
                key = string.Format("{0}:{1}", _cacheKeyPrefix, key);
            string data = DefaultDB.HashGet(key, "value");
            if (string.IsNullOrEmpty(data))
            {
                return default(T);
            }
            if (extendCacheTime)
            {
                var cacheTime = (long)DefaultDB.HashGet(key, "timeout");
                DefaultDB.KeyExpire(key, TimeSpan.FromMilliseconds(cacheTime));
            }
            if (typeof(T).IsValueType)
            {
                return (T)ConvertValue(typeof(T), data);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(data.ToString());
            }
        }
        #endregion

        #region ConvertValue
        private static object ConvertValue(Type type, string value)
        {
            TryConvertValue(type, value, out object result, out Exception error);
            if (error != null)
            {
                throw error;
            }
            return result;
        }

        private static bool TryConvertValue(Type type, string value, out object result, out Exception error)
        {
            error = null;
            result = null;
            if (type == typeof(object))
            {
                result = value;
                return true;
            }
            if (type.GetTypeInfo().IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (string.IsNullOrEmpty(value))
                {
                    return true;
                }
                return TryConvertValue(Nullable.GetUnderlyingType(type), value, out result, out error);
            }
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (converter.CanConvertFrom(typeof(string)))
            {
                try
                {
                    result = converter.ConvertFromInvariantString(value);
                }
                catch (Exception innerException)
                {
                    error = innerException;
                }
                return true;
            }
            return false;
        }

        #endregion

        #region Set
        /// <summary>
        /// 写缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void Set<T>(string key, T data)
        {
            Set(key, data, _expire,true);
        }
        /// <summary>
        /// 写缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void Set<T>(string key, T data, bool addPrefix)
        {
            Set(key, data, _expire, addPrefix);
        }
        /// <summary>
        /// 写缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="addPrefix">key是否添加前缀</param>
        public void Set<T>(string key, T data, int cacheTime, bool addPrefix)
        {
            if (addPrefix)
                key = string.Format("{0}:{1}", _cacheKeyPrefix, key);
            InnerSet(key, data, cacheTime);
        }
        /// <summary>
        /// 写缓存
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="cacheTime">过期时间（分钟）</param>
        public void Set<T>(string key, T data, int cacheTime)
        {
            Set(key,data,cacheTime,true);
        }
        private void InnerSet<T>(string key, T data, int cacheTime)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key不能为空字符串或null");
            var value = typeof(T).Name.ToLower() == "char" ? data.ToString() : JsonConvert.SerializeObject(data);
            HashEntry[] entrys = new HashEntry[] {
             new HashEntry("value",value),
             new HashEntry("expiration",1),
             new HashEntry("created",DateTime.Now.Ticks),
             new HashEntry("defaultExpiration",1),
             new HashEntry("version",1),
             new HashEntry("type",data ==null?"":data.GetType().FullName+","+data.GetType().Assembly.FullName),
             new HashEntry("timeout",cacheTime*60*1000),
            };
            DefaultDB.HashSet(key, entrys);
            if (cacheTime > 0)
            {
                DefaultDB.KeyExpire(key, TimeSpan.FromMinutes(cacheTime));
            }
        }
        #endregion

        #region Replace
        /// <summary>
        /// 替换key-value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="data"></param>
        public void Replace<T>(string key, T data)
        {
            Replace(key, data, _expire,true);
        }

        public void Replace<T>(string key, T data, bool addPrefix)
        {
            Replace(key, data, _expire, addPrefix);
        }

        public void Replace<T>(string key, T data, int cacheTime)
        {
            Replace(key, data, cacheTime,true);
        }

        public void Replace<T>(string key, T data, int cacheTime, bool addPrefix)
        {
            if (addPrefix)
                key = string.Format("{0}:{1}", _cacheKeyPrefix, key);
            Set(key, data, cacheTime);
        }
        #endregion

        #region SearchKeys
        /// <summary>
        /// 模糊匹配key 。
        /// </summary>
        /// <param name="pattern">可包含通配符的key值，如【*userId_*】->匹配包含【userId_】的所有key</param>
        /// <returns></returns>
        public IList<string> SearchKeys(string pattern)
        {
            var redisResult = DefaultDB.ScriptEvaluate(LuaScript.Prepare("  return  redis.call('KEYS', @keypattern) "), new { @keypattern = pattern });
            string[] preSult = (string[])redisResult;//将返回的结果集转为数组
            return preSult?.ToList();
        }
        #endregion

        #region HasKey
        /// <summary>
        /// 检查key是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool HasKey(string key)
        {
           return HasKey(key,true);
        }

        public bool HasKey(string key, bool addPrefix)
        {
            if (addPrefix)
                key = string.Format("{0}:{1}", _cacheKeyPrefix, key);
            return DefaultDB.KeyExists(key);
        }
        #endregion

        #region Remove
        /// <summary>
        /// 删除指定key
        /// </summary>
        /// <param name="key"></param>
        public void Remove(string key)
        {
            Remove(key,true);
        }

        public void Remove(string key, string prefix)
        {
            key = string.Format("{0}:{1}", prefix, key);
            DefaultDB.KeyDelete(key);
        }

        public void Remove(string key, bool addPrefix)
        {
            if (addPrefix)
                Remove( key, _cacheKeyPrefix);
            else
                DefaultDB.KeyDelete(key);
        }
        #endregion

        #region BulkRemove
        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="key">通配key值，如【userId_*】->匹配以【userId_】开头的所有key</param>
        /// <param name="pageSize">页码数</param>
        public long BulkRemove(string key, int pageSize = 5000)
        {
            var keys = Conn.GetServer(Conn.GetEndPoints()[0]).Keys(database: DefaultDB.Database, pattern: key, pageSize: pageSize);
            return DefaultDB.KeyDelete(keys.ToArray());
        }
   
        #endregion

        #region RemoveAll
        /// <summary>
        /// 清理整个db数据 
        /// </summary>
        public void RemoveAll()
        {
            DefaultDB.Execute(" flushdb ");
        }
        #endregion

        #region GetLock
        /// <summary>
        /// 获取Redis分布式事务锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expireAt">自动释放时间（毫秒）</param>
        /// <returns></returns>
        public IDisposable GetLock(string key, double expireAt)
        {
            var token = System.Environment.MachineName;
            return GetLock( key, token, expireAt);
        }
        /// <summary>
        /// 获取Redis分布式事务锁
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <param name="expireAt">自动释放时间（毫秒）</param>
        /// <returns></returns>
        public RedisLockTake GetLock(string key, string token, double expireAt)
        {
            return new RedisLockTake(DefaultDB, key, token, expireAt);
        }

        #endregion

        #region set集合数据类型的操作
        /// <summary>
        /// 获取整个set集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="addPrefix"></param>
        /// <returns></returns>
        public List<T> SGet<T>(string key, bool addPrefix = true)
        {
            if (addPrefix)
            {
                key = $"{_cacheKeyPrefix}:{key}";
            }
            var data = DefaultDB.SetMembers(key);
            return JsonConvert.DeserializeObject<List<T>>(data.ToJson());
        }

        public bool SAdd<T>(string key, T value, bool addPrefix = true)
        {
            if (addPrefix)
            {
                key = $"{_cacheKeyPrefix}:{key}";
            }           
            return DefaultDB.SetAdd(key, value.ToJson());
        }

        /// <summary>
        /// 判断key下的set中是否包含指定值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="addPrefix"></param>
        /// <returns></returns>
        public bool SContains<T>(string key, T value, bool addPrefix = true)
        {
            if (addPrefix)
            {
                key = $"{_cacheKeyPrefix}:{key}";
            }

            return DefaultDB.SetContains(key, value.ToJson());
        }
        /// <summary>
        /// 从指定集合中移除itemValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="addPrefix"></param>
        /// <returns></returns>
        public bool SRemove<T>(string key, T value, bool addPrefix = true)
        {
            if (addPrefix)
            {
                key = $"{_cacheKeyPrefix}:{key}";
            }
            return  DefaultDB.SetRemove(key,value.ToJson());
        }

        public T SPop<T>(string key, bool addPrefix = true)
        {
            if (addPrefix)
            {
                key = $"{_cacheKeyPrefix}:{key}";
            }

            var data= DefaultDB.SetPop(key); 
            if (typeof(T).IsValueType)
            {
                return (T)ConvertValue(typeof(T), data);
            }
            else
            {
                return JsonConvert.DeserializeObject<T>(data.ToString());
            }
        }
        #endregion set集合数据类型的操作

        public string Hosts { set; get; }

        private ConnectionMultiplexer Conn { get; set; }

        private IDatabase DefaultDB
        {
            get { return Conn.GetDatabase(); }
        }
        public RedisType RedisType { get; }

    }
}
