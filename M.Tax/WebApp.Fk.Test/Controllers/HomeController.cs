﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Fk.Test.Controllers
{
    public class HomeController : Controller
    {
        static string Now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        public ActionResult Index()
        {
            ViewBag.Now = Now;
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

            return View();
        }
    }
}