using Microsoft.AspNetCore.Http;
using System;

namespace Tax.Common.Extention
{
    public static class HttpContextExtention
    {

        #region cookie操作
        private const string CookieKey = "token";

        /// <summary>
        /// 从cookie中拿到信息并解密
        /// </summary>
        /// <param name="accessor"></param>
        /// <returns></returns>
        public static string GetCookie(this HttpContext httpContext, string cookieKey = CookieKey)
        {
            if (string.IsNullOrWhiteSpace(cookieKey))
            {
                return "";
            }
            return DEncrypt.Decrypt(httpContext.Request.Cookies[cookieKey]);
        }

        /// <summary>
        /// 将信息加密保存到cookie
        /// </summary>
        /// <param name="accessor"></param>
        /// <param name="cookieValue"></param>
        /// <returns></returns>
        public static bool SetCookie(this HttpContext httpContext, string cookieValue, string cookieKey = CookieKey)
        {
            if (string.IsNullOrWhiteSpace(cookieValue) || string.IsNullOrWhiteSpace(cookieKey))
            {
                return false;
            }
            httpContext.Response.Cookies.Append(CookieKey, DEncrypt.Encrypt(cookieValue), new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true,
                Secure = false
            });

            return true;
        }

        public static void DeleteCookie(this HttpContext httpContext,  string cookieKey = CookieKey)
        {
            httpContext.Response.Cookies.Delete(cookieKey);
        }
        #endregion
    }
}
