using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Tax.AdminWeb.Filters;
using Tax.AdminWeb.Hubs;
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
            services.AddControllersWithViews();

            services.AddMvc(options =>
               {
                   options.Filters.Add<ExceptionProcessFilter>();
                   options.Filters.Add<RequestCheckFilter>();
               }).SetCompatibilityVersion( CompatibilityVersion.Latest);

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton(new RepositoryOption(BaseCore.Configuration.GetConnectionString("TaxDB")));
            services.AddSingleton<UsersRepository>();
            services.AddSingleton<StaticFilesRepository>();
            services.AddSingleton<ClientMenusRepository>();
            services.AddSingleton<UsersService>();
            services.AddSingleton<StaticFilesService>();
            services.AddSingleton<ClientMenusService>();
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
            //新增uploadimg静态资源文件目录
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), BaseCore.Configuration["ImgPath:savePath"])),
                RequestPath = "/"+ BaseCore.Configuration["ImgPath:savePath"]
            }) ; 
            //新增tempimg上传文件临时目录
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), BaseCore.Configuration["ImgPath:tempPath"])),
                RequestPath = "/"+ BaseCore.Configuration["ImgPath:tempPath"]
            });
            //app.UseCookiePolicy();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute("areas","{area:exists}/{controller}/{action=Index}");
                endpoints.MapControllerRoute("default0", "default/{area=Console}/{controller=user}/{action=Index}");
                endpoints.MapControllerRoute("default", "{area=Console}/{controller=user}/{action=Index}");
                endpoints.MapControllerRoute("home", "home/{area=Console}/{controller=Menus}/{action=Index}");
                //引入signalr中心
                endpoints.MapHub<ChatHub>("/chathub");
            });
        }
    }
}
