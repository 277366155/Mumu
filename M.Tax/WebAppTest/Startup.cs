using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebAppTest.Hubs;
using Tax.Common;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using Microsoft.Extensions.Configuration;

namespace WebAppTest
{
    public class Startup
    {
        public IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient();
            services.AddMvc();
            services.AddSignalR();
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(BaseCore.AppSetting["SecurityKey"]));
            services.AddSingleton(securityKey);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)//cookie默认认证方案
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt => {
                    //opt.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//是否验证issuer前发者
                        ValidateAudience = true,//是否验证audience客户端
                        ValidateLifetime = true,//是否验证失效时间
                        ClockSkew = TimeSpan.FromSeconds(30),//失效后的30s内仍可使用
                        ValidAudience="boo",
                        ValidIssuer="boo",
                        IssuerSigningKey=securityKey//获取当前的私钥
                    };
                });
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
            ////注册顺序，且要在useEndpoints之前
            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "zhuye/{controller=Home}/{action=Index}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");

                endpoints.MapHub<ChatHub>("/chathub");
            });

            this.Configuration.ConsulRegist();

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
