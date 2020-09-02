using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tax.Model.ParamModel;
using Tax.Service;

namespace Tax.AdminWeb.Areas.Console.Controllers
{
    public class SlideController : BaseController
    {
        readonly StaticFilesService _staticFilesService;
        public SlideController(StaticFilesService staticFilesService)
        {
            _staticFilesService = staticFilesService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileAsync()
        {
              var data= await _staticFilesService.UploadFileAsync(HttpContext);
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveSlideAsync(SaveSlideParam param)
        {
          var data =await  _staticFilesService.SaveSlideAsync(param);
            return Json(data); 
        }

        [HttpPost]
        public async Task<IActionResult> GetSlidesPager(PagerParam pager)
        {
            var data =await _staticFilesService.GetPagerListAsync(pager);
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSlide(int id)
        {
            var data = await _staticFilesService.DeleteSlideAsync(id);
            return Json(data); 
        }
    }
}