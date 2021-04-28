using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebAppTest
{
    [Route("api/[controller]")]
    [ApiController]
    public class APITestController : ControllerBase
    {
        [HttpGet("getdata/{id}")]
        public IActionResult GetData(int id)
        {
            //Thread.Sleep(1000);
            return new JsonResult(new { id=id, name="webappTest"});
        }
    }
}
