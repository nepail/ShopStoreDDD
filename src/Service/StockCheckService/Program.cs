#region �����c�vʷ�޸�����

/*
    ����:Windows Service Worker
    ��������:2022-01-10

    ����:���� Hosting �׼��Ԇ��� WindowsService
    �޸�����:2022-01-12

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
