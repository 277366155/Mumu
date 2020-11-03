using StackExchange.Redis;
using System;

namespace SissCloud.Caching.Redis
{
    /// <summary>
    /// redis分布式事务锁
    /// </summary>
    public class RedisLockTake : IDisposable
    {
        private IDatabase _db;
        string _token;
        string _key;

        public bool LockResult { get; private set; }
        /// <summary>
        /// redis锁
        /// </summary>
        /// <param name="db">database</param>
        /// <param name="key">锁键</param>
        /// <param name="token">锁值</param>
        /// <param name="expire">锁自动释放时间(s)</param>
        public RedisLockTake(IDatabase db,string key,string token, double expire)
        {
            if (db != null)
            {
                _db = db;
                _key = key;
                _token = token;
                LockResult = db.LockTake(key, this._token, TimeSpan.FromSeconds(expire));
            }
        }
        public void Dispose()
        {
            if (_db != null&&LockResult)
            {
                _db.LockRelease(_key, _token);
            }
        }
    }
}
