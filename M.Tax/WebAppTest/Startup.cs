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

            ////use()��ע��һ���������м��������ָ��next��һ����ʲô
            ////run()��ִ��action���Ѿ����м��ĩ�ˣ�����ִ�к�����м����
            //app.Use(async (context,next)=> {
            //    await next.Invoke();
            //    await context.Response.WriteAsync("This is response msg.");                
            //});

            //app.UseMyTestMiddleware();
            ////��������ַ�д���test��������ִ��builder.run�е�ί��
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
