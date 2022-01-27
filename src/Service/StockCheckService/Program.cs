#region 功能與歷史修改描述

/*
    描述:Windows Service Worker
    建立日期:2022-01-10

    描述:加入 Hosting 套件以啟用 WindowsService
    修改日期:2022-01-12

 */

#endregion


using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ShopStoreWorkerService
{
    public class Program
    {

        public static void Main(string[] args)
        {            
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    //services.AddHostedService<JwtSVC>();
                    services.AddSingleton<JwtSVC>();
                });
    }
}
