using AutoMapper;
using System;
using System.Collections.Generic;
using Tax.Common;
using Tax.Common.Extention;
using Tax.Model.DBModel;
using Tax.Model.ParamModel;
using Tax.Model.ViewModel;

namespace Tax.Service
{
    public static class AutoMapperConfig
    {
        public static TDestination Map<TSource, TDestination>(this TDestination destination, TSource source)
        {
            return Mapper.Map(source, destination);
        }

        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                ////CreateMap<TestEntity.Models.Person, Person>().AfterMap((source, destination) =>
                ////{
                ////    destination.Brithday = source.Brithday.ToString();
                ////    destination.Balance = Convert.ToDouble(source.Balance);
                ////}).ForAllMembers(p => {
                ////    p.Condition((s, d, sMember) => {
                ////        return sMember != null;
                ////    });
                ////});

                //幻灯片显示名同标题
                cfg.CreateMap<SaveStaticFilesParamBase, StaticFiles>().AfterMap((s,d)=> {                     
                    d.ShowName = s.Title;
                    d.Extensions = s.SavePath.Substring(s.SavePath.LastIndexOf('.'));
                    d.Version = DateTime.Now.ToMilliTicks().ToString();
                });
                cfg.CreateMap<StaticFiles, Slide>().AfterMap((s, d) => {
                    d.Path =$"/{BaseCore.Configuration["ImgPath:SavePath"]}/{ s.SavePath}";
                });

                cfg.CreateMap<Pager<StaticFiles>, Pager<Slide>>().AfterMap((s,d)=> {
                    d.DataList = Mapper.Map<List<Slide>>(s.DataList);
                });
                cfg.CreateMap<SaveMenuParam, ClientMenus>().AfterMap((s, d) => {
                    d.Version = DateTime.Now.ToMilliTicks().ToString();
                    d.Enable = true;
                });
                
            });
        }
    }
}
