using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Formatting.Compact;
using Tax.Common;

namespace WebAppTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(BaseCore.Configuration)
            //    .MinimumLevel.Debug()
            //    .Enrich.FromLogContext()
            //    .WriteTo.Console(new RenderedCompactJsonFormatter())
            //    .WriteTo.File(formatter: new CompactJsonFormatter(), "logs\\log.txt", rollingInterval: RollingInterval.Day)
            //    .CreateLogger();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
           .UseConfiguration(BaseCore.Configuration)
            .UseStartup<Startup>()
            .UseSerilog(dispose: true);
    }
}
