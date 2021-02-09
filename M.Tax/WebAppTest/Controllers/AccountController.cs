using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication;

namespace WebAppTest.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return Content("account index data .");
        }
        //默认通过cookie认证
        [Authorize]
        public async Task<IActionResult> ByCookie()
        {
            return Content("get data by cookie info ,name : "+User.FindFirstValue("Name"));
        }

        /// <summary>
        /// 通过jwtToken认证
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes =JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ByJwt()
        {
            return Content("ByJwt name:" + User.FindFirstValue("Name"));
        }
        /// <summary>
        /// 通过jwt和cookie均可
        /// </summary>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme+","+CookieAuthenticationDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ByAny()
        {
            return Content("ByAny name:" + User.FindFirstValue("Name"));
        }

        /// <summary>
        /// 生成token信息
        /// </summary>
        /// <param name="securityKey"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<IActionResult> CreateJwtToken([FromServices] SymmetricSecurityKey securityKey, string userName)
        {
            var claims = new List<Claim>();
            claims.Add(new Claim("Name", userName));
            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: "boo",
                audience: "boo",
                claims: claims,
                expires: DateTime.Now.AddMinutes(40),
                signingCredentials: creds);
            var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
            return Content(tokenStr);
        }

        /// <summary>
        /// 设置cookie信息
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task<IActionResult> CreateCookie(string userName)
        {
            var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            identity.AddClaim(new Claim("Name",userName));
           await   HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
            return Content("set cookie is ok");
        }
    }
}
