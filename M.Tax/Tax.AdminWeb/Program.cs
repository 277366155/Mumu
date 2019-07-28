using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Tax.Common;
using Tax.Service;

namespace Tax.AdminWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Trace.Listeners.Add(new CustomTraceListener());
            AutoMapperConfig.Configure();
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
           .UseConfiguration(BaseCore.Configuration)
            //.UseUrls(BaseCore.Configuration["urls"])            
            .UseStartup<Startup>();
    }
}
