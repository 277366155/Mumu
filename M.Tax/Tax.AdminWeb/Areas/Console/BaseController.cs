using Microsoft.AspNetCore.Mvc;
using Tax.AdminWeb.Filters;

namespace Tax.AdminWeb.Areas.Console
{
    [IdentityCheck]
    [Area("Console")]
    public class BaseController: Controller
    {
    }
}
