using AutoMapper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Tax.Common;
using Tax.Model;
using Tax.Model.DBModel;
using Tax.Model.Enum;
using Tax.Model.ParamModel;
using Tax.Model.ViewModel;
using Tax.Repository;

namespace Tax.Service
{
    public class StaticFilesService
    {
        readonly StaticFilesRepository _staticFilesRep;

        public StaticFilesService(StaticFilesRepository staticFilesRep)
        {
            _staticFilesRep = staticFilesRep;
        }

        /// <summary>
        /// 保存幻灯片文件：更新和新增
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<BaseResult> SaveSlideAsync(SaveSlideParam param)
        {
            var exsit = await _staticFilesRep.Count($" and id ={param.Id}") > 0;
            if (!exsit)
            {
                return await SaveUploadFileAsync(param);
            }
            else
            {
                return await UpdateSlideAsync(param);
            }
        }

        /// <summary>
        /// 更新Slide
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<BaseResult> UpdateSlideAsync(SaveSlideParam param)
        {
            var fileName = param.SavePath.Substring(param.SavePath.LastIndexOf('/'));
            var data = await _staticFilesRep.GetModel(param.Id);
            //文件不存在则是新上传的
            if (!File.Exists(Directory.GetCurrentDirectory() + $"/{BaseCore.Configuration["ImgPath:savePath"]}/"+fileName))
            {
                MoveFile(fileName);
                //同时删除原图片。。
                DeleteFile(data.SavePath);
            }
            //只保存文件名
            param.SavePath = param.SavePath.Substring(param.SavePath.LastIndexOf('/') + 1);
            var updateResult = await _staticFilesRep.Update(Mapper.Map<StaticFiles>(param));
            if (updateResult > 0)
            {
                return new Success("保存成功");
            }
            else
            {
                return new Fail("操作失败");
            }
        }
        #region 文件上传
        public async Task<BaseResult> SaveUploadFileAsync(SaveSlideParam param)
        {
            MoveFile(param.SavePath.Substring(param.SavePath.LastIndexOf('/')));
            //只保存文件名
            param.SavePath =  param.SavePath.Substring(param.SavePath.LastIndexOf('/')+1);
            var insertResult = await _staticFilesRep.InsertFile(Mapper.Map<StaticFiles>(param));
            if (insertResult > 0)
            {
                return new Success("保存成功");
            }
            else
            {
                return new Fail("操作失败");
            }
        }

        /// <summary>
        /// 将指定文件从临时目录移动到正式目录
        /// </summary>
        /// <param name="fileName"></param>
        public void MoveFile(string fileName)
        {
            var saveDirPath = Directory.GetCurrentDirectory() + $"/{BaseCore.Configuration["ImgPath:savePath"]}/";
            var tempPath = Directory.GetCurrentDirectory() + $"/{BaseCore.Configuration["ImgPath:tempPath"]}/";
            if (!File.Exists(tempPath + fileName))
            {
                throw new Exception($"文件不存在：[{tempPath + fileName}]"); //return false;
            }

            if (!Directory.Exists(saveDirPath))
            {
                Directory.CreateDirectory(saveDirPath);
            }
            //文件已存在
            if (File.Exists(saveDirPath + fileName))
            {
                throw new Exception($"文件已存在：[{saveDirPath + fileName}]");
            }
            File.Move(tempPath + fileName, saveDirPath + fileName);
        }

        public void DeleteFile(string fileName)
        {
            var saveDirPath = Directory.GetCurrentDirectory() + $"/{BaseCore.Configuration["ImgPath:savePath"]}/";
            if (File.Exists(saveDirPath + fileName))
            {
                File.Delete(saveDirPath + fileName);
            }
        }

        public async Task<Result<string>> UploadFileAsync()
        {
            var result = new Result<string>() { IsSuccessed = false, Data = "", Msg = "上传失败" };
            string fullName = "";
            string dirTempPath = Directory.GetCurrentDirectory() + $"/{BaseCore.Configuration["ImgPath:tempPath"]}/";
            long maxLength = 1024 * 1024 * 30;//最大30m
            var files = BaseCore.CurrentContext.Request.Form.Files;
            if (files.Count <= 0)
            {
                result.Msg = "请选择上传文件";
                return result;
            }
            if (files[0].Length <= 0)
            {
                result.Msg = "请选择上传文件";
                return result;
            }
            //后缀名
            string extName = Path.GetExtension(files[0].FileName).ToLower();
            string fileName = Guid.NewGuid().ToString();
            //存储全名
            fullName = fileName + extName;

            string imageFilter = ".jpg|.jpeg|.png|.gif|.bmp";
            if (!imageFilter.Contains(extName.ToLower()))
            {
                result.Msg = "请上传正确的图片格式（.jpg|.jpeg|.png|.gif|.bmp）";
                return result;
            }
            if (files[0].Length > maxLength)
            {
                result.Msg = "文件最大不能超过30MB";
                return result;
            }
   
            if (!Directory.Exists(dirTempPath))
            {
                Directory.CreateDirectory(dirTempPath);
            }
            var filePath = dirTempPath + fullName;
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await files[0].CopyToAsync(stream);
            }
            result.IsSuccessed = true;
            result.Msg = "上传成功";
            result.Data = $"/{BaseCore.Configuration["ImgPath:tempPath"]}/" + fullName;
            return result;
        }
        #endregion 文件上传

        public async Task<Pager<Slide>> GetPagerListAsync(PagerParam pager)
        {
         var data =await  _staticFilesRep.GetPageList($"  and Type={(int)FileType.Slide} ",pager.PageIndex,pager.PageSize,null,"sortID ,Title");
            return Mapper.Map<Pager<Slide>>(data);
        }

        public async Task<BaseResult> DeleteSlideAsync(int id)
        {
            var data = await _staticFilesRep.GetModel(id);
            var deleteResult = await _staticFilesRep.Delete(id);
            if (deleteResult > 0)
            {
                DeleteFile(data.SavePath);
                return new Success("删除成功");
            }
            else
            {
                return new Fail("操作失败");
            }
        }
    }
}
