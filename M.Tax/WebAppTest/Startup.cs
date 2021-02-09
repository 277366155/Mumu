using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebAppTest.Hubs;
using WebAppTest.Middlewares;
using Microsoft.Extensions.Http;

namespace WebAppTest
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddMvc();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "home/{controller=Home}/{action=Index}");

                endpoints.MapHub<ChatHub>("/chathub");
            });

            ////use()是注入一个完整的中间件。可以指定next下一步做什么
            ////run()是执行action，已经是中间件末端，不再执行后面的中间件。
            //app.Use(async (context,next)=> {
            //    await next.Invoke();
            //    await context.Response.WriteAsync("This is response msg.");                
            //});

            //app.UseMyTestMiddleware();
            ////如果请求地址中带有test参数，就执行builder.run中的委托
            //app.MapWhen(context=> {
            //    return context.Request.Query.ContainsKey("test");
            //}, builder=> {
            //    builder.Run(async context=> {
            //        await context.Response.WriteAsync(" contains 'test' key. ");
            //    });
            //});
            //app.MapWhen(context => {
            //    return context.Request.Query.ContainsKey("boo");
            //}, builder => {
            //    builder.Run(async context => {
            //        await context.Response.WriteAsync(" contains 'boo' key.");
            //    });
            //});

        }
    }
}
