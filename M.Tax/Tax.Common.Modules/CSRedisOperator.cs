using CSRedis;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tax.Common
{
    public class CSRedisOperator
    {
        CSRedisClient csRedisClient;
        public CSRedisOperator(string redisConfig)
        {
            csRedisClient = new CSRedisClient(redisConfig);
        }

        public void HSet<T>(string key, T obj)
        {
            var list = new List<object>();
            list.Add("value");
            list.Add(obj);
            list.Add("created");
            list.Add(DateTime.Now.Ticks);
            csRedisClient.HMSet(key,list.ToArray());
            csRedisClient.Expire(key,6000);
        }

        public T HGet<T>(string key)
        {
          return   csRedisClient.HGet<T>(key, "value");
        }
    }
}
