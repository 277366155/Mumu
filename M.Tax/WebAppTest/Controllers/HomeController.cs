using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tax.Common;

namespace WebAppTest.Controllers
{
    public class HomeController : Controller
    {
        IHttpClientFactory _httpClientFactory;
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ESTest()
        {
            return Json(new { name = "test", age = 22 });
        }

        [HttpGet]
        public async Task<IActionResult> RequestAsync()
        {

                var taskList = new List<Task>();
            //for (var i = 0; i < 100; i++)
            //{
            //    Console.WriteLine(i);

            //    var result = await client.GetAsync("https://localhost:44378/home/GetOrSetSession");
            //    if (result.Headers.TryGetValues("Set-Cookie", out IEnumerable<string> cookie))
            //    {
            //        Console.WriteLine(cookie.ToJson());
            //        Console.WriteLine("响应结果：" + result.Content.ReadAsStringAsync().Result);
            //    }
            //    else
            //    {
            //        Console.WriteLine("响应结果：" + result.Content.ReadAsStringAsync().Result);
            //    }
            //}


            for (var j = 0; j < 20; j++)
            {
                Task.Run(async () =>
                {
                    for (var i = 1; i > 0; i++)
                    {
                        var url = "https://localhost:44378/home/GetOrSetSession";
                        if (i % 2 == 0)
                        {
                            url = "https://localhost:44378/home/GetOrSetSession2";
                        }
                        var client = _httpClientFactory.CreateClient(Guid.NewGuid().ToString());
                        var result = await client.GetAsync(url);
                        Console.WriteLine("当前请求次数：" + i + "。返回结果：" + result.Content.ReadAsStringAsync().Result);
                    }
                });
            }
            return Json("ok");
        }
    }
}
