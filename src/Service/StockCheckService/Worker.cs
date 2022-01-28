#region 功能與歷史修改描述

/*
    描述:Windows Service Worker
    日期:2022-01-10

    描述:程式碼風格調整
    日期:2022-01-21

    描述:加入JwtToken Generator
    日期:2022-01-21

 */

#endregion

using Dapper;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;


namespace ShopStoreWorkerService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> LOGGER;
        private readonly string LOGPATH;
        private StreamWriter CPULOGGER = null!;
        private readonly HubConnection CONNECTION;        
        private readonly string CONNSTR = "Data Source=localhost;Initial Catalog=ShoppingDB;User ID=shopstoreadmin;Password=pk!shopstoreadmin;Integrated Security=false;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        private readonly int CHECKTIME;                

        public Worker
        (
            ILogger<Worker> logger,
            IConfiguration config,
            JwtSVC jwt
        )
        {
            LOGGER = logger;
            LOGPATH = Path.Combine(config.GetValue<string>("LogPath") ?? AppContext.BaseDirectory!, "cpu2.log");           
            CONNECTION = new HubConnectionBuilder().WithUrl(config.GetValue<string>("DomainUrl"), options =>
            {                       
                //產生Token
                options.AccessTokenProvider = () => Task.FromResult(jwt.GenerateToken("Worker"));

                //The SSL connection could not be established, see inner exception.'
                //應用程式進行SSL連線時會驗證SSL的安全性，測試環境為本機，加入以下設定略過
                options.WebSocketConfiguration = conf =>
                {
                    conf.RemoteCertificateValidationCallback = (message, cert, chain, errors) => { return true; };
                };

                options.HttpMessageHandlerFactory = factory => new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; }
                };

            }).Build();
            
            CHECKTIME = config.GetValue<int>("CheckTime");
            //DomainUrl = config.GetValue<string>("DomainUrl");
        }

        //服務啟動
        public override async Task StartAsync(CancellationToken stoppingToken)
        {                                   
            CPULOGGER = new StreamWriter(LOGPATH, true);            
            await CONNECTION.StartAsync();
            LOGGER.LogInformation("connection successful");
            LOGGER.LogInformation("Service started");
            
            // 基底類別 BackgroundService 在 StartAsync() 呼叫 ExecuteAsync、
            // 在 StopAsync() 時呼叫 stoppingToken.Cancel() 優雅結束
            await base.StartAsync(stoppingToken);
        }

        void Log(string message)
        {
            if (CPULOGGER == null) return;
            CPULOGGER.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
            CPULOGGER.Flush();
        }

        int GetCpuLoad()
        {
            using var p = new Process();
            p.StartInfo.FileName = "wmic.exe";
            p.StartInfo.Arguments = "cpu get loadpercentage";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            int load = -1;
            var m = System.Text.RegularExpressions.Regex.Match(
                p.StandardOutput.ReadToEnd(), @"\d+");
            if (m.Success) load = int.Parse(m.Value);
            p.WaitForExit();
            return load;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                LOGGER.LogInformation("Worker running at: {time}", DateTimeOffset.Now);                
                await Task.Delay(1000, stoppingToken);

                // 使用 ThreadPool 執行，避免讀取 CPU 百分比的耗用時間干擾 Task.Delay 間隔
                // https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads
                // 這裡用舊式 ThreadPool 寫法，亦可用 Task 取代 
                // 參考：https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-cancel-a-task-and-its-children
                ThreadPool.QueueUserWorkItem(
                    (obj) =>
                    {
                        try
                        {
                            var cancelToken = (CancellationToken)obj!;
                            if (!stoppingToken.IsCancellationRequested)
                            {

                                //var cpuValue = GetCpuLoad();
                                //Log($"CPU: {cpuValue}%");
                                //_logger.LogInformation($"Logging CPU load");
                                //_logger.LogInformation($"CPU: {cpuValue}%");                                

                                using var conn = new SqlConnection(CONNSTR);
                                var result = conn.QueryMultiple("pro_bg_checkProducts",
                                commandType: System.Data.CommandType.StoredProcedure);

                                var item = result.Read<InventoryViewModel>().ToDictionary(x => x.Stock, x => x);
                                var logg = JsonConvert.SerializeObject(item);
                                //_logger.LogInformation(logg);
                                CONNECTION.InvokeAsync("SendMessage", "test:", logg);
                            }
                        }
                        catch (Exception ex)
                        {
                            LOGGER.LogError(ex.ToString());
                            throw;
                        }
                    }, stoppingToken);
                await Task.Delay(CHECKTIME, stoppingToken);
            }
        }

        private async void SendCPUUsage(int cpu)
        {
            await CONNECTION.InvokeAsync("SendMessage", "CPU:", cpu.ToString());
        }

        // 服務停止
        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            LOGGER.LogInformation("Service stopped");
            Log("Service stopped");
            CPULOGGER.Dispose();
            CPULOGGER = null!;
            await base.StopAsync(stoppingToken);
        }
    }
}
