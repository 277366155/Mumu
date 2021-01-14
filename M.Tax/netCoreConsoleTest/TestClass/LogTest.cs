using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;
using Tax.Common;
using Microsoft.Extensions.Configuration;

namespace netCoreConsoleTest
{
    public class LogTest
    {

        public static void LogBuild(string[] args)
        {
            //接收命令行的配置信息
            BaseCore.InitConfigurationBuilder(a=>a.AddCommandLine(args));
            var service = new ServiceCollection();
            service.AddLogging(builder =>
            {
                builder.AddConfiguration(BaseCore.Configuration.GetSection("Logging"));
                builder.AddConsole();
                //需添加nuget引用：Microsoft.Extensions.Logging.Debug
                builder.AddDebug();
            });
            service.AddSingleton<ServiceTest>();
            var sp = service.BuildServiceProvider();

            //#region 使用服务注册的方式（一般使用这种方式）
            ///*
            // * 日志名称默认为: 命名空间+类名
            // * 需要在配置文件中配置日志名对应的日志输出级别
            // */
            //var serviceLog = sp.GetService<ServiceTest>();
            //serviceLog.LogTest();
            //#endregion 使用服务注册的方式

            //#region 使用createLogger的方式
            //var loggerFactory = sp.GetService<ILoggerFactory>();
            //var testLogger = loggerFactory.CreateLogger("testLogger");
            //testLogger.LogInformation(199, "testLogger的information");
            //testLogger.LogWarning(200, "testLogger warning 信息");
            //testLogger.LogError(new Exception("testLogger的错误"), "testLogger报错了..");

            //#endregion 使用createLogger的方式

            #region log作用域
            var scopeLogger = sp.GetService<ILogger<ServiceTest>>();
            //scope相当于创建了一个上下文的串联id
            using (scopeLogger.BeginScope("ScopeId:{scopeId}", Guid.NewGuid()))
            {
                scopeLogger.LogInformation("scopeLogger的information");
                scopeLogger.LogError("scopeLogger的error");
            }
            #endregion log作用域
        }
    }

    public class ServiceTest
    {
        static ILogger<ServiceTest> _logger;
        public ServiceTest(ILogger<ServiceTest> logger)
        {
            _logger = logger;
        }
        public void LogTest()
        {
            _logger.LogInformation("ServiceTest：info信息");
            _logger.LogError("ServiceTest：错误信息");
        }
    }
}
