#region �����c�vʷ�޸�����

/*
    ����:Startup
    ����:2022-01-27

 */

#endregion

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
                //��C���õ����Q��
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                //����ָ������C���á� ��δ��C��ʹ����Ҫ����Ҫ��C�Ķ��c�r�����ܕ��l����C����
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                //JWT ��������C�����ɏ� �ڙ� Ҫ����^�н≺�s����C JWT ���ȁ������C��
                //ȡ�û��O���Á���C�R�e���ȵą�����

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier,
                    RoleClaimType = ClaimTypes.Role,

                    //�_����C���ȵđ��ó�ʽ�����Á�����ȵĽ�耡� ��һ�����ⰸ�������еĽ�考�Ƕ�ڙ����С� ���@�N��rͨ�������l����
                    ValidateIssuerSigningKey = true,                    
                    IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("fkadsf;pdfddksssfq")),

                    ValidateIssuer = false, // ����C���l��
                    ValidateAudience = false  // ����C Audience (Token���շ�)
                };
            });


            services.AddSingleton<ConUserService>();            

            //SignalR �A�O�_��JsonProtocol
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

            app.UseCors(builder =>
            {
                builder.WithOrigins("http://localhost:6372")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                //.WithMethods("GET", "POST")
                .AllowCredentials();
            });

            //�� QueryString �� AccessToken ����Ո����^��
            app.Use(async (context, next) =>
            {
                if (context.Request.Path.Value.StartsWith("/chatHub") || context.Request.Path.Value.StartsWith("/ServerHub"))
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
