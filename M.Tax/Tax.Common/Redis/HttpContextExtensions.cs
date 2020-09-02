using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using Tax.Common;
using Tax.Common.Extention;

namespace SissCloud.Caching.Redis
{
    public static class HttpContextExtensions
    {
        readonly static string _domain = BaseCore.AppSetting["Domain"];
        readonly static string _sessionPrefix = "CustomSession";

        /// <summary>
        /// sessionid在cookie中name
        /// </summary>
        readonly static string _sessionIdCookieName = "SessionId";
        /// <summary>
        /// session的默认过期时间（分钟）
        /// </summary>
        readonly static int _sessionTimeOut = 30;
        /// <summary>
        /// 根据sessionId和key值返回相应的RedisKey。key为空时返回sessionId下的模糊匹配key值（如:SissUserSession:S65HGIN52B*）
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetSessionRedisKey(string sessionId, string key)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return "";
            }
            if (string.IsNullOrWhiteSpace(key))
            {
                return $"{_sessionPrefix}:{sessionId}*";
            }
            return $"{_sessionPrefix}:{sessionId}:{key}";
        }
        /// <summary>
        /// 生成SessionId的cookie信息,key为空时不存储session信息
        /// </summary>
        /// <param name="context"></param>
        public static void CreateRedisSession(this HttpContext context, string key=null,object value=null)
        {
            var sessionId = Guid.NewGuid().ToString();
            var cookieOpt = new CookieOptions
            {
                Expires = DateTime.Now.AddDays(30)
            };
            if (!_domain.IsNullOrWhiteSpace())
            {
                cookieOpt.Domain = _domain;
            }
            context.Response.Cookies.Append(_sessionIdCookieName, sessionId,cookieOpt);

            if (!string.IsNullOrWhiteSpace(key))
            {
                context.SetRedisSession(key,value);
            }
        }
        /// <summary>
        /// 设置基于当前http会话的缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="act">缺失sessionid时要执行的操作</param>
        public static void SetRedisSession<T>(this HttpContext context, string key, T value, Action act= null)
        {
            var sessionId = context.Request.Cookies[_sessionIdCookieName];
            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                RedisHelper.CacheAccount.Set(GetSessionRedisKey(sessionId, key), value, _sessionTimeOut * 60);
            }
            else
            {
                if (act == null)
                {
                    throw new Exception("缺失sessionId信息");
                }
                else
                {
                    act.Invoke();
                }
            }
          
        }

        /// <summary>
        /// 获取当前会话中的缓存信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T GetRedisSession<T>(this HttpContext context, string key)
        {
            var sessionId = context.Request.Cookies[_sessionIdCookieName];
            //cookie中不存在sessionId，则新增一个sessionId
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return default(T);
            }
            return RedisHelper.CacheAccount.Get<T>(GetSessionRedisKey(sessionId, key));
        }

        /// <summary>
        /// 清空当前会话中的所有session缓存
        /// </summary>
        /// <param name="context"></param>
        /// <param name="key"></param>
        public static void ClearRedisSession(this HttpContext context, string key = null)
        {
            var sessionId = context.Request.Cookies[_sessionIdCookieName];
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return;
            }           
            var keyStr =  GetSessionRedisKey(sessionId, key);
            RedisHelper.CacheAccount.BulkRemove(keyStr);            
        }
    }
}
