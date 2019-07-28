using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Tax.Common;
using Tax.Common.Extention;
using Tax.Model;
using Tax.Model.DBModel;
using Tax.Model.ParamModel;
using Tax.Repository;

namespace Tax.Service
{
    public class UsersService
    {
        readonly UsersRepository _usersRep;
        public UsersService(UsersRepository usersRep)
        {
            _usersRep = usersRep;
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<BaseResult> LoginCheck(LoginParam param)
        {
            param.Password = DEncrypt.Encrypt(param.Password);
            var data = await _usersRep.FirstOrDefault(" and UserName=@UserName and Password=@Password", param);
            if (data !=null)
            {
                var expirationTime = DateTime.Now.AddHours(1);
                var tokenVaule = $"{param.UserName}|{expirationTime.Ticks}";
               BaseCore.CurrentContext.SetCookie(tokenVaule);
                SetUserInfoSession(param.UserName, data);
                return new Success("登陆成功");
            }
            else
            {
                return new Fail("用户名或密码错误");
            }
        }

        public async Task<bool> InserUser(InsertUserParam param)
        {
            param.Password = DEncrypt.Encrypt(param.Password);
            return _usersRep.InsertUser(param).Result > 0;
        }

        /// <summary>
        /// 根据当前用户cookie，判断是否登录
        /// </summary>
        /// <returns></returns>
        public static Users GetUserInfoSession(HttpContext context)
        {
            var token = context.GetCookie();
            if (token.IsNullOrWhiteSpace())
            {
                return null;
            }
            var userName = token.Split('|')[0];
            var ticks = token.Split('|')[1];
            if (DateTime.Now.Ticks > Convert.ToInt64(ticks))
            {
                context.DeleteCookie();
                return null;
            }
            return CacheTools.GetData<Users>(userName );
        }

        public static void SetUserInfoSession(string userName, Users  userInfo)
        {           
            CacheTools.SetData(userName, userInfo);
        }
    }
}
