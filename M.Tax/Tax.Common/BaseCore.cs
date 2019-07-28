﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System;
using System.IO;

namespace Tax.Common
{
    public class BaseCore
    {
        public static IServiceProvider ServiceProvider { get; set; }
        public static IHttpContextAccessor CurrentAccessor { get; set; }

        public static IHostingEnvironment CurrentEnvironment
        {
            get
            {
                object factory = ServiceProvider.GetService(typeof(IHostingEnvironment));
                return (IHostingEnvironment)factory;
            }
        }

        public static HttpContext CurrentContext
        {
            get
            {
                return CurrentAccessor.HttpContext;
            }
        }


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
    }
}
