using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tax.Common;
using Tax.Model.ParamModel;
using Tax.Service;

namespace Tax.AdminWeb.Areas.Console.Controllers
{
    public class PCController : BaseController
    {
        readonly StaticFilesService _staticFilesService;
        readonly ClientMenusService _clientMenusService;
        public PCController(ClientMenusService clientMenusService, StaticFilesService staticFilesService)
        {
            _clientMenusService = clientMenusService;
            _staticFilesService = staticFilesService;
        }


        #region 欢迎图片
        public async Task<IActionResult> WelcomeImg()
        {
            var data  = await _staticFilesService.GetWelcomeImgFileAsync();
            if (data != null)
            {
                data.SavePath = "/"+BaseCore.Configuration.GetSection("ImgPath:savePath").Value+"/"+ data.SavePath;
                ViewBag.WelcomeImg = data;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveWelcomeImgAsync(SaveWelcomeImgParam param)
        {
            var data = await _staticFilesService.SaveStaticFileAsync(param);
            return Json(data);
        }
        #endregion

        #region 菜单背景图片
        public async Task<IActionResult> MenuBackgroundImg()
        {
            var data = await _staticFilesService.GetMenuBackgroundImgFileAsync();
            if (data != null)
            {
                data.SavePath = "/" + BaseCore.Configuration.GetSection("ImgPath:savePath").Value + "/" + data.SavePath;
                ViewBag.BackgroundImg = data;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SaveBackgroundImgAsync(SaveBackgroundImgParam param)
        {
            var data = await _staticFilesService.SaveStaticFileAsync(param);
            return Json(data);
        }
        #endregion

        #region 菜单管理
        public IActionResult Menus()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> GetMenusPager(PagerParam pager)
        {
            var data = await _clientMenusService.GetPageListAsync(pager);
            return Json(data);
        }

        [HttpPost]
        public async Task<IActionResult> SaveMenuAsync(SaveMenuParam param)
        {
            var data = await _clientMenusService.SaveMenuAsync(param);
            return Json(data);
        }
        #endregion
    }
}