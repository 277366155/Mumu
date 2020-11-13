using System;
using Tax.Common;

namespace netCoreConsoleTest
{
    class Program
    {
        static void Main(string[] args)
        {
            BaseCore.ConfigurationOnChange(() => Console.WriteLine("json config has changed."));
            Console.Read();
        }

    }
}
