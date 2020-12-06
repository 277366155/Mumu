using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Tax.Common;
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
        public async Task<BaseResult> LoginCheck(LoginParam param,HttpContext context)
        {
            param.Password = DEncrypt.Encrypt(param.Password);
            var data = await _usersRep.FirstOrDefaultAsync(" and UserName=@UserName and Password=@Password", param);
            if (data !=null)
            {
                context.SetCookie(param.UserName);
                SetUserInfoSession(param.UserName, data);
                return new Success("登陆成功");
            }
            else
            {
                return new Fail("用户名或密码错误");
            }
        }

        public BaseResult Logout(HttpContext httpContext)
        {
            var userName= httpContext.GetCookie();
            httpContext.DeleteCookie();
            DeleteUserInfoSession(userName);

            return new Success();
        }
        public async Task<bool> InserUser(InsertUserParam param)
        {
            param.Password = DEncrypt.Encrypt(param.Password);
            return (await _usersRep.InsertUserAsync(param)) > 0;
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
            var user= CacheTools.GetData<Users>(token);
            if (user != null)
            {
                //失效时间顺延
                SetUserInfoSession(token, user);
            }
            return user;
        }

        public static void SetUserInfoSession(string userName, Users  userInfo)
        {           
            CacheTools.SetData(userName, userInfo);
        }

        public static void DeleteUserInfoSession(string userName)
        {
            CacheTools.DeleteData(userName);
        }
    }
}
