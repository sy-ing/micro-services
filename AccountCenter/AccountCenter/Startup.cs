using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AccountCenter.AppCode;
using AccountCenter.Models.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AccountCenter
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

            //注册数据库连接字符串
            string s = Configuration.GetConnectionString("DefaultConnection");

            Method.ContextStr = s;

            services.AddDbContext<ContextString>(options =>
options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            ////注册服务器地址
            Method.ServerAddr = Configuration.GetConnectionString("ServerAddress");


            //添加跨域访问配置
            services.AddCors(options => options.AddPolicy("CorsSample",
        p => p.WithOrigins("*").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin().AllowCredentials()));


           


            string IsStartHF = Configuration.GetConnectionString("HangfireStart");
            //添加hangfire服务
            //if (IsStartHF == "true")
            //{
            //    services.AddHangfire(x => x.UseSqlServerStorage(s));
            //}


            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            var redisConn = Configuration["WebConfig:Redis:Connection"];
            RedisHelper redisHelper = new RedisHelper(redisConn);
            Method._RedisHelper = redisHelper;

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, ContextString context, IDistributedCache cache)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            //启用跨域访问配置
            app.UseCors("CorsSample");

            app.UseMiddleware<IdentityMiddleware>();

            app.UseHttpsRedirection();

            //启用静态文件
            var staticfile = new StaticFileOptions();
            staticfile.ServeUnknownFileTypes = true;
            staticfile.DefaultContentType = "application/x-msdownload"; //设置默认  MIME
            var provider = new FileExtensionContentTypeProvider();
            provider.Mappings.Add(".log", "text/plain");//手动设置对应MIME
            staticfile.ContentTypeProvider = provider;
            app.UseStaticFiles(staticfile);


            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            //初始化数据库
            DbInitializer.Initialize(context);
            Method._hostingEnvironment = env;
           // Method._context = context;
        }


      
    }
}
