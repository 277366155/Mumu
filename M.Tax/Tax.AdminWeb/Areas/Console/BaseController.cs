using Microsoft.AspNetCore.Mvc;
using Tax.AdminWeb.Filters;
using Tax.Common;

namespace Tax.AdminWeb.Areas.Console
{
    [IdentityCheck]
    [Area("Console")]
    public class BaseController: Controller
    {         
    }
}
