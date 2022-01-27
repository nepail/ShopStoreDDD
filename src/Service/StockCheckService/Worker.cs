#region �����c�vʷ�޸�����

/*
    ����:Windows Service Worker
    ��������:2022-01-10

    ����:��ʽ�a�L���{��
    �޸�����:2022-01-21

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
        private readonly string DOMAINURL;
        private readonly JwtSVC JWTSVC;


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
                //options.Cookies.Add(new System.Net.Cookie("manager", "123"));            


                //�a��Token
                options.AccessTokenProvider = () => Task.FromResult(jwt.GenerateToken("Worker"));

                //The SSL connection could not be established, see inner exception.'
                //���ó�ʽ�M��SSL�B���r����CSSL�İ�ȫ�ԣ��yԇ�h���鱾�C�����������O�����^
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

        // ���Ն��ӕr
        public override async Task StartAsync(CancellationToken stoppingToken)
        {

            
            //var con = new CookieContainer()
            
            CPULOGGER = new StreamWriter(LOGPATH, true);            
            await CONNECTION.StartAsync();
            LOGGER.LogInformation("connection successful");
            LOGGER.LogInformation("Service started");
            
            // ����e BackgroundService �� StartAsync() ���� ExecuteAsync��
            // �� StopAsync() �r���� stoppingToken.Cancel() ���ŽY��
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

                // ʹ�� ThreadPool ���У������xȡ CPU �ٷֱȵĺ��Õr�g�ɔ_ Task.Delay �g��
                // https://docs.microsoft.com/en-us/dotnet/standard/threading/cancellation-in-managed-threads
                // �@�e���fʽ ThreadPool ����������� Task ȡ�� 
                // ������https://docs.microsoft.com/en-us/dotnet/standard/parallel-programming/how-to-cancel-a-task-and-its-children
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

        // ����ֹͣ
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
