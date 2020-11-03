using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tax.AdminWeb.Filters;
using Tax.Common;
using Tax.Common.Extention;

namespace Tax.AdminWeb.Areas.Console.Controllers
{
    public class MenusController : BaseController
    {
        public IActionResult Index()
        {
            var token= HttpContext.GetCookie();
            if (!string.IsNullOrWhiteSpace(token))
            {
                ViewBag.UserName = token.Split('|')[0];
            }
            return View();
        }

        public IActionResult Statistics()
        {
            return View();
        }
    }
}