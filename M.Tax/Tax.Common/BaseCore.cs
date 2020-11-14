﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Diagnostics;
using System.IO;

namespace Tax.Common
{
    public class BaseCore
    {
        private static IConfigurationBuilder _builder;
        private static object lockObj = new object();
        public static IConfigurationBuilder InitConfigurationBuilder(Action<IConfigurationBuilder> act = null)
        {

            if (_builder == null)
            {
                lock (lockObj)
                {
                    if (_builder == null)
                    {
                        var builder = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
                        _builder = builder;
                    }
                }
            }
            act?.Invoke(_builder);

            return _builder;
        }

        /// <summary>
        /// 读取配置
        /// </summary>
        public static IConfigurationRoot Configuration
        {
            get
            {
                return InitConfigurationBuilder().Build();
            }
        }

        /// <summary>
        /// 配置文件修改时触发
        /// </summary>
        /// <param name="act"></param>
        public static void ConfigurationOnChange(Action act)
        {
            if (act == null)
                act = () => { Trace.WriteLine("appsettings.json was changed"); };

            ChangeToken.OnChange(()=>Configuration.GetReloadToken(), act);
        }

        /// <summary>
        /// 读取根节点下的appsetting节点
        /// </summary>
        public static IConfigurationSection AppSetting
        {
            get
            {
                return Configuration.GetSection("AppSetting");
            }
        }
    }
}
