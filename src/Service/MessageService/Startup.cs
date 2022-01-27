using MessageService.Hubs;
using MessageService.Hubs.Models.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace MessageService
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
            //services.AddRazorPages();

            services.AddAuthentication(options =>
            {
                //驗證配置的名稱。
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //挑戰指定的驗證配置。 當未驗證的使用者要求需要驗證的端點時，可能會發出驗證挑戰。
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                //JWT 持有人驗證會藉由從 授權 要求標頭中解壓縮和驗證 JWT 權杖來執行驗證。
                //取得或設定用來驗證識別權杖的參數。

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role,

                    //確保驗證權杖的應用程式信任用來簽署權杖的金鑰。 有一個特殊案例，其中的金鑰內嵌在權杖中。 但這種情況通常不會發生。
                    ValidateIssuerSigningKey = true,
                    //IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c")),
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("fkadsf;pdfddksssfq")),

                    ValidateIssuer = false, // 不驗證簽發者
                    ValidateAudience = false  // 不驗證 Audience (Token接收方)
                };

                //options.Events = new JwtBearerEvents()
                //{
                //    OnMessageReceived = context =>
                //    {
                //        var accessToken = context.Request.Query["access_token"];

                //        var path = context.HttpContext.Request.Path;
                //        if(!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chatHub"))
                //        {
                //            context.Token = accessToken;
                //        }

                //        return System.Threading.Tasks.Task.CompletedTask;
                //    }
                //};


            });


            services.AddSingleton<ConUserService>();            

            //SignalR 預設開啟JsonProtocol
            services.AddSignalR().AddJsonProtocol();

            services.AddCors();
        }

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

            //app.UseStaticFiles();

            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:6372")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                //.WithMethods("GET", "POST")
                .AllowCredentials();
            });


            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.StartsWith("/chatHub"))
                {
                    var bearerToken = context.Request.Query["access_token"].ToString();

                    if (!string.IsNullOrEmpty(bearerToken))
                        context.Request.Headers.Add("Authorization", "Bearer " + bearerToken);
                }

                await next();
            });


            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<ChatHub>("/chatHub");
                endpoints.MapHub<ServerHub>("/ServerHub");
            });
        }
    }
}
