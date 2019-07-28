using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Tax.Service;

namespace Tax.AdminWeb.Filters
{
    public class IdentityCheckAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {
           
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            
          var userInfo=  UsersService.GetUserInfoSession(context.HttpContext);
            if (userInfo == null)
            {
                context.Result = new RedirectResult($"/default");
            }
        }
    }
}
