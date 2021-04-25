using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApp.Fk.Test.ApolloConfig;

namespace WebApp.Fk.Test.Controllers
{
    public class HomeController : Controller
    {
        static string Now = DateTime.Now.ToString("北京时间 [ yyyy-MM-dd HH:mm:ss ]");
        //static ApolloConfiguration apollo = new ApolloConfiguration();
        static string appid = ConfigurationManager.AppSettings["Apollo.AppID"];

        public ActionResult Index()
        {
            ApolloConfiguration apollo = new ApolloConfiguration();
            var key = "appName";
            var publicKey = "redisAddress";
            var configVal= apollo.GetValue(publicKey, "system");
            ViewBag.Config = configVal;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = $"{AppConfig.appid}";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "联系方式页面.";

            return View();
        }

        public class AppConfig
        {
          public  static string appid = ConfigurationManager.AppSettings["Apollo.AppID"];
        }
    }
}