using Microsoft.AspNetCore.Mvc;
using Tax.AdminWeb.Filters;
using Tax.Common;
using Tax.Common.Extention;

namespace Tax.AdminWeb.Areas.Console
{
    [IdentityCheck]
    [Area("Console")]
    public class BaseController: Controller
    {         
    }
}
