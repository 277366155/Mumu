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

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)//cookieĬ����֤����
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, opt => {
                    //opt.ExpireTimeSpan = TimeSpan.FromMinutes(60);
                })
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opt =>
                {
                    opt.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,//�Ƿ���֤issuerǰ����
                        ValidateAudience = true,//�Ƿ���֤audience�ͻ���
                        ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
                        ClockSkew = TimeSpan.FromSeconds(30),//ʧЧ���30s���Կ�ʹ��
                        ValidAudience="boo",
                        ValidIssuer="boo",
                        IssuerSigningKey=securityKey//��ȡ��ǰ��˽Կ
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
            ////ע��˳����Ҫ��useEndpoints֮ǰ
            //app.UseAuthentication();
            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("default", "zhuye/{controller=Home}/{action=Index}");
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}");

                endpoints.MapHub<ChatHub>("/chathub");
            });

            this.Configuration.ConsulRegist();

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
