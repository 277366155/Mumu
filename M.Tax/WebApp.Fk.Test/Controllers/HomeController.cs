using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Fk.Test.Controllers
{
    public class HomeController : Controller
    {
        static string Now = DateTime.Now.ToString("北京时间 [ yyyy-MM-dd HH:mm:ss ]");
        public ActionResult Index()
        {
            ViewBag.Now =  Now;
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "介绍页面.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "联系方式页面.";

            return View();
        }
    }
}