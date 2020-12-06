using Microsoft.Extensions.Configuration;
using System;
using Tax.Common;

namespace netCoreConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            LogTest.LogBuild();
            Console.Read();
        }

        static void ConfigOnChangeTest()
        {
            BaseCore.ConfigurationOnChange(() => Console.WriteLine("json config has changed."));
        }
    }
}

