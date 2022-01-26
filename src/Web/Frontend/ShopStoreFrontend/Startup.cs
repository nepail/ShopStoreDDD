#region �����c�vʷ�޸�����

/*
    ����:Startup ����
    ����:2021-11-17

    ����:��ʽ�a�L���{��
    ����:2022-01-20

    ����:�]��MinIO��HttpClient��MinIOSVC���գ��޸ĳ�ʽ�]��
    ����:2022-01-25

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
using ShopStore.Common;
using ShopStore.Common.Filters;
using ShopStoreFrontend.Domain.Models.Interface;
using ShopStoreFrontend.Domain.Service;
using ShopStoreFrontend.Persistence.Models.Service;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http;

namespace ShopStore
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
            double LoginExpireMinute = this.Configuration.GetValue<double>("LoginExpireMinute");
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
                //option.InstanceName = "MyWebSite_"; //Redis ��ǰ�Y�ִ�
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


            services.AddScoped<ActionFilter>();
            services.AddScoped<AuthorizationFilter>();

            //��̨�����aƷ�a��MD5�a���� DataProtection API����Ҫ�����@�μӽ��܃�����g����tIIS�����e
            services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(@"D:\DataProtection\"));                 

            services.AddSession(option =>
            {
                //�O�����ڕr�g
                //option.IdleTimeout = TimeSpan.FromMinutes(30);
            });

            //������ӆ���ڙ��^�V��
            services.AddMvc(option =>
            {
                option.Filters.Add<ActionFilter>();
                option.Filters.Add<AuthorizationFilter>();
            });

            //���É��s�ؑ�
            services.AddResponseCompression(option =>
            {
                //ͬ�r���� Gzip �� Brotil���s
                option.Providers.Add<BrotliCompressionProvider>();
                option.Providers.Add<GzipCompressionProvider>();
                option.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "image/svg+xml" });
            });

            services.Configure<BrotliCompressionProviderOptions>(option =>
            {
                //�Զ��x���s���e
                option.Level = (CompressionLevel)5;
            });

            services.AddMemoryCache();

            //SignalR �A�O�_��JsonProtocol
            services.AddSignalR().AddJsonProtocol();
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

            //�����ȡ�O��
            //app.UseStaticFiles(new StaticFileOptions
            //{
            //    OnPrepareResponse = ctx =>
            //    {
            //        ctx.Context.Response.Headers[HeaderNames.CacheControl] = "public, max-age=86400";
            //    }            
            //});            

            //���É��s�ؑ�
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
                //����ȫ����C
                .RequireAuthorization();                
            });
        }
    }
}
