using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tax.Model;

namespace Tax.AdminWeb.Filters
{
    public class RequestCheckFilter : IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            //参数验证
            if (!context.ModelState.IsValid)
            {
                var result = new Fail() { Msg=""};
                foreach (var item in context.ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        result.Msg += error.ErrorMessage + "，";
                    }
                }
                result.Msg = result.Msg.TrimEnd('，');
                context.Result = new JsonResult(result);
            }
        }
    }
}
