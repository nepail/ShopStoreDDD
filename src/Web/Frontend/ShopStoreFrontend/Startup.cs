#region 功能與歷史修改描述

/*
    描述:Startup 配置
    日期:2021-11-17

    描述:程式碼風格調整
    日期:2022-01-20

    描述:註冊MinIO、HttpClient、MinIOSVC服務，修改程式註解
    日期:2022-01-25

 */

#endregion

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Minio;
using ShopStoreFrontend.Domain.Models.Interface;
using ShopStoreFrontend.Domain.Service;
using ShopStoreFrontend.Filters;
using ShopStoreFrontend.Persistence.Models.Service;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;

namespace ShopStoreFrontend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            double LoginExpireMinute = Configuration.GetValue<double>("LoginExpireMinute");
            string connectionString = Configuration["SqlConStr"];

            services
                .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(option =>
                {
                    option.LoginPath = new PathString("/Member/Login");
                    option.LogoutPath = new PathString("/Member/Logout");
                    option.AccessDeniedPath = new PathString("/Home/AccessDenied");
                });

            services.AddStackExchangeRedisCache(option =>
            {
                option.Configuration = Configuration.GetSection("Redis")["ConnectionString"];
                //option.InstanceName = "MyWebSite_"; //Redis 的前綴字串
            });

            services.AddControllersWithViews();

            services.AddTransient<IProducts, ProductsSVC>();
            services.AddTransient<IMembers, MembersSVC>();
            services.AddTransient<ICart, CartSVC>();
            services.AddTransient<IOrders, OrderSVC>();
            services.AddTransient(e => new SqlConnection(connectionString));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton(e => new MinioClient(Configuration["MinIO:Endpoint"], Configuration["MinIO:AccessKey"], Configuration["MinIO:SecretKey"]));
            services.AddSingleton<MinIOSVC>();
            services.AddSingleton<JwtSVC>();      
            
            services.AddScoped<AuthorizationFilter>();

            //後台新增產品產生MD5碼呼叫 DataProtection API，需要加上這段加解密儲存空間，否則IIS會報錯
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"D:\DataProtection\"));

            services.AddSession(option =>
            {
                //設定逾期時間
                //option.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            //加入自訂的授權過濾器
            services.AddMvc(option =>
            {                
                option.Filters.Add<AuthorizationFilter>();
            });

            //啟用壓縮回應
            services.AddResponseCompression(option =>
            {
                //同時啟用 Gzip 及 Brotil壓縮
                option.Providers.Add<BrotliCompressionProvider>();
                option.Providers.Add<GzipCompressionProvider>();
                option.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });

            services.Configure<BrotliCompressionProviderOptions>(option =>
            {
                //自定義壓縮級別
                option.Level = (CompressionLevel)5;
            });

            services.AddMemoryCache();            
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();         

            //啟用壓縮回應
            app.UseResponseCompression();

            app.UseAuthentication();

            app.UseRouting();

            app.UseSession();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}")
                //啟用全域驗證
                .RequireAuthorization();
            });
        }
    }
}
