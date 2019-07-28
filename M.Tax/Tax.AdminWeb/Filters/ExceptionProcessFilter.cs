using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using Tax.Common.Extention;

namespace Tax.AdminWeb.Filters
{
    public class ExceptionProcessFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            Trace.TraceError(context.Exception.ToJson());
        }
    }
}
