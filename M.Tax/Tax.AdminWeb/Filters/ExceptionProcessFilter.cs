using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using Tax.Common.Extention;
using Tax.Model;

namespace Tax.AdminWeb.Filters
{
    public class ExceptionProcessFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            Trace.TraceError(context.Exception.ToJson());
            context.Result = new JsonResult(new Fail() { Msg = "服务器内部错误" });          
        }
    }
}
