using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Tax.AdminWeb.Filters;
using Tax.Common;
using Tax.Model;
using Tax.Model.ParamModel;
using Tax.Service;

namespace Tax.AdminWeb.Areas.Console.Controllers
{
    [Area("Console")]
    public class UserController : Controller
    {
        readonly UsersService _userSer;
        public UserController(UsersService userSer)
        {
            EnableCookie();
            _userSer = userSer;
        }
        private void EnableCookie()
        {
            var consentFeature = HttpContext.Features.Get<ITrackingConsentFeature>();
            consentFeature.GrantConsent();
        }
        public IActionResult Index()
        {
            var userInfo=UsersService.GetUserInfoSession(HttpContext);
            if(userInfo!=null)
            {
                return Redirect("/home");
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult<BaseResult>> Login([FromForm]LoginParam loginParam)
        {
            var result = await _userSer.LoginCheck(loginParam,HttpContext);
            return Json(result);
        }

        [IdentityCheck]
        [HttpPost]
        public ActionResult<BaseResult> Logout()
        {
          return Json(_userSer.Logout(HttpContext));
        }
    }
}