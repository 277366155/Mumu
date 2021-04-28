using Microsoft.AspNetCore.Mvc;
using System.Threading;

namespace Tax.AdminWeb.Areas
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiTestController : ControllerBase
    {
        [HttpGet("getdata/{id}")]
        public IActionResult GetData(int id)
        {
            //Thread.Sleep(1000);
            return new JsonResult(new { id=id,name="test result"});
        }
    }
}
