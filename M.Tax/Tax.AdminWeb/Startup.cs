using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Tax.AdminWeb.Filters;
using Tax.Common;
using Tax.Repository;
using Tax.Service;

namespace Tax.AdminWeb
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc(options =>
               {
                   options.Filters.Add<ExceptionProcessFilter>();
                   options.Filters.Add<RequestCheckFilter>();
               })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(new RepositoryOption(BaseCore.Configuration.GetConnectionString("TaxDB")));
            services.AddSingleton<UsersRepository>();
            services.AddSingleton<StaticFilesRepository>();
            services.AddSingleton<UsersService>();
            services.AddSingleton<StaticFilesService>();

            BaseCore.ServiceProvider = services.BuildServiceProvider();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IHttpContextAccessor accessor)
        {
            BaseCore.CurrentAccessor = accessor;
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            var savePath = Directory.GetCurrentDirectory() +"/"+ BaseCore.Configuration["ImgPath:savePath"];
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            var tempPath = Directory.GetCurrentDirectory() + "/" + BaseCore.Configuration["ImgPath:tempPath"];
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            //����uploadimg��̬��Դ�ļ�Ŀ¼
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), BaseCore.Configuration["ImgPath:savePath"])),
                RequestPath = "/"+ BaseCore.Configuration["ImgPath:savePath"]
            }) ; 
            //����tempimg�ϴ��ļ���ʱĿ¼
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), BaseCore.Configuration["ImgPath:tempPath"])),
                RequestPath = "/"+ BaseCore.Configuration["ImgPath:tempPath"]
            });
            app.UseCookiePolicy();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                   name: "areas",
                   template: "{area:exists}/{controller}/{action=Index}"
                 );
                routes.MapRoute("default0", "default/{area=Console}/{controller=user}/{action=Index}");
                routes.MapRoute("default", "{area=Console}/{controller=user}/{action=Index}");
                routes.MapRoute("home", "home/{area=Console}/{controller=Menus}/{action=Index}");
            });
        }


    }
}
