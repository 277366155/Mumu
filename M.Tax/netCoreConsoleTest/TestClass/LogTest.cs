using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Logging;
using Tax.Common;

namespace netCoreConsoleTest
{
    public class LogTest
    {

        public static void LogBuild()
        {
            var service = new ServiceCollection();
            service.AddLogging(builder =>
            {
                builder.AddConfiguration(BaseCore.Configuration.GetSection("Logging"));
                builder.AddConsole();
            });
            service.AddSingleton<ServiceTest>();
            var sp = service.BuildServiceProvider();

            #region 使用服务注册的方式（一般使用这种方式）
            /*
             * 日志名称默认为: 命名空间+类名
             * 需要在配置文件中配置日志名对应的日志输出级别
             */
            var serviceLog = sp.GetService<ServiceTest>();
            serviceLog.InformationLogTest();
            # endregion 使用服务注册的方式
            
            #region 使用createLogger的方式
            var loggerFactory = sp.GetService<ILoggerFactory>();
            var testLogger = loggerFactory.CreateLogger("testLogger");
            testLogger.LogWarning(200, "boo");
            testLogger.LogError(new Exception("错误"),"error..");
            #endregion 使用createLogger的方式
        }
    }

    public class ServiceTest
    {
        static ILogger<ServiceTest> _logger;
        public ServiceTest(ILogger<ServiceTest> logger)
        {
            _logger = logger;
        }
        public void InformationLogTest()
        {
            _logger.LogInformation("测试日志信息");
        }
    }
}
