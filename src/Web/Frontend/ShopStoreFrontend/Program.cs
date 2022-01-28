#region 功能與歷史修改描述

/*
    描述:Program 配置
    建立日期:2021-11-17

    描述:程式碼風格調整
    修改日期:2022-01-20

    描述:新增 Docker 設定 & 加入註解
    修改日期:2022-01-21

 */

#endregion

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace ShopStoreFrontend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                    //發佈時若勾選單一檔案，需修改使用 IISIntegration 啟用 Out-Procces 模式
                    //.UseIISIntegration() //Out-Proccess
                    //.UseIIS() In-Proccess
                    .UseStartup<Startup>();

                    //Docker 內運行需指定內部端口80，否則會出錯
                    //.UseUrls("http://*:80");
                    //.UseUrls("http://192.168.6.4:5051");
                });
    }
}
