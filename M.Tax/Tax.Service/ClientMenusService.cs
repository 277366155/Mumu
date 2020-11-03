using AutoMapper;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tax.Common;
using Tax.Common.Extention;
using Tax.Model;
using Tax.Model.DBModel;
using Tax.Model.Enum;
using Tax.Model.ParamModel;
using Tax.Model.ViewModel;
using Tax.Repository;

namespace Tax.Service
{
    public class ClientMenusService
    {

        readonly ClientMenusRepository _clientMenusRep;
        readonly StaticFilesRepository _staticFilesRep;
        readonly StaticFilesService _staticFilesSer;
        public ClientMenusService(ClientMenusRepository clientMenusRep, StaticFilesRepository staticFilesRep, StaticFilesService staticFilesSer)
        {
            _clientMenusRep = clientMenusRep;
            _staticFilesRep = staticFilesRep;
            _staticFilesSer = staticFilesSer;
        }

        /// <summary>
        /// 新增或者更新菜单信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<BaseResult> SaveMenuAsync(SaveMenuParam param)
        {

            //检查当前menu是否存在
            var model =await  _clientMenusRep.GetModelAsync(param.ID.Value);
            if (model == null)
            {
                return await InsertIconAndMenuInfoAsync(param);
            }
            else
            {
                //更新
               return  await UpdateIconAndMenuInfoAsync(param,model);
            }
        }

        /// <summary>
        /// 新增菜单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private async Task<BaseResult> InsertIconAndMenuInfoAsync(SaveMenuParam param)
        {
            var icon = Mapper.Map<StaticFiles>(param.MenuIcon);
            using (var conn = ClientMenusRepository.CreateMysqlConnection())
            {
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        var iconId = await _staticFilesRep.InsertFileAsync(icon, tran);
                        if (iconId <= 0)
                        {
                            throw new Exception("图标信息保存失败");
                        }
                        var menuInfo = Mapper.Map<ClientMenus>(param);
                        var insertMenuResult = await _clientMenusRep.InsertMenuAsync(menuInfo, tran);
                        if (insertMenuResult <= 0)
                        {
                            throw new Exception("菜单信息保存失败");
                        }

                        tran.Commit();
                        return new Success("操作成功");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Trace.TraceError(ex.Message);
                        return new Fail("操作失败：" + ex.Message);
                    }
                }
            }
        }

        /// <summary>
        /// 修改菜单
        /// </summary>
        /// <param name="param"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        private async Task<BaseResult> UpdateIconAndMenuInfoAsync(SaveMenuParam param, ClientMenus model)
        {
            var iconInfo = await _staticFilesRep.GetModelAsync(model.IconFileID);
            param.MenuIcon.Id = param.IconFileID.Value;
            //文件不同，需要重新保存修改
            if (param.MenuIcon.SavePath.Substring(param.MenuIcon.SavePath.LastIndexOf('/') + 1) != iconInfo.SavePath)
            {
                _staticFilesSer.DeleteFile(iconInfo.SavePath);
            }
            using (var conn = StaticFilesRepository.CreateMysqlConnection())
            {
                using (var tran = conn.BeginTransaction())
                {
                    try
                    {
                        //根据id更新图片信息
                        param.MenuIcon.SavePath = param.MenuIcon.SavePath.Substring(param.MenuIcon.SavePath.LastIndexOf('/') + 1);
                        param.MenuIcon.CreateTime = iconInfo.CreateTime;
                        //param.MenuIcon.Title = iconInfo.Title;
                        var updateResult = await _staticFilesRep.UpdateAsync(Mapper.Map<StaticFiles>(param.MenuIcon), tran);
                        if (updateResult <= 0)
                        {
                            throw new Exception("图标信息保存失败");
                        }

                        var menuUpdateResult = await _clientMenusRep.UpdateAsync(Mapper.Map<ClientMenus>(param),tran);
                        if(menuUpdateResult<=0)
                        {
                            throw new Exception("菜单信息保存失败");
                        }
                        tran.Commit();
                        return new Success("操作成功");
                    }
                    catch (Exception ex)
                    {
                        tran.Rollback();
                        Trace.TraceError(ex.Message);
                        return new Fail("操作错误："+ex.Message);
                    }
                }
            }

        }

        /// <summary>
        /// 获取菜单列表分页
        /// </summary>
        /// <param name="pager"></param>
        /// <returns></returns>
        public async Task<Pager<Menu>> GetPageListAsync(PagerParam pager)
        {
            var data = await _clientMenusRep.GetMenuPageListAsync($" and c.ClientType={(int)ClientTypeEnum.PC}", pager.PageIndex, pager.PageSize,null," c.SortId,c.ID asc");
            if (data.DataList != null && data.DataList.Count > 0)
            {
                var imgDir =  BaseCore.Configuration["ImgPath:savePath"]  ;
                data.DataList.ForEach(a =>
                {
                    if (!a.IconPath.IsNullOrWhiteSpace())
                    {
                        a.IconPath = $"/{imgDir}/{a.IconPath}";
                    }
                });
            }
            return data;
        }
    }
}
