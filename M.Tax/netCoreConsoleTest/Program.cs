using Microsoft.Extensions.Configuration;
using System;
using Tax.Common;

namespace netCoreConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            LogTest.LogBuild(args);

            //new DBTractionTest().TranTest();
            Console.WriteLine("开启消息推送。。");
            RabbitMQTest.PublisherTest();

            Console.Read();
        }

        static void ConfigOnChangeTest()
        {
            BaseCore.ConfigurationOnChange(() => Console.WriteLine("json config has changed."));
        }
    }
}

