using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebAppFrameWork.Log4Net;

namespace WebAppFrameWork.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            LogHelper.Info("测试info日志记录");
            LogHelper.Error("测试error日志记录");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";
            Response.Headers.Add("tenantid","222");
            return View();
        }
    }
}