#region 功能與歷史修改描述

/*
    描述:檔案儲存服務
    日期:2022-01-25    
    
 */

#endregion

using Microsoft.AspNetCore.Http;
using Minio;
using Minio.Exceptions;
using ShopStoreBackend.Domain.Models.Manager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using static ShopStoreBackend.Domain.Service.MinIOSVC.UserAlert;

namespace ShopStoreBackend.Domain.Service
{
    public class MinIOSVC
    {
        private readonly string BUCKETNAME = "shopstore";
        private readonly MinioClient MINIO;

        public MinIOSVC(MinioClient minio)
        {
            MINIO = minio;
        }

        /// <summary>
        /// 上傳檔案
        /// </summary>
        /// <param name="file">檔案實體</param>
        /// <param name="id">檔案名稱</param>
        public async Task<string> UploadFile(IFormFile file, string id, string content)
        {

            try
            {
                //確認儲存桶是否存在
                bool found = await MINIO.BucketExistsAsync(BUCKETNAME);

                if (!found)
                {
                    await MINIO.MakeBucketAsync(BUCKETNAME);
                }

                //上傳圖片
                if (file.Length > 0)
                {
                    Stream stream = file.OpenReadStream();
                    await MINIO.PutObjectAsync(BUCKETNAME, @$"/product/img/{id}.jpg", stream, file.Length, file.ContentType);
                }

                //上傳文字內容
                if (content != null)
                {
                    var stream = new MemoryStream();
                    using var writer = new StreamWriter(stream);
                    writer.Write(content);
                    writer.Flush();
                    stream.Position = 0;
                    await MINIO.PutObjectAsync(BUCKETNAME, @$"/product/content/{id}.txt", stream, stream.Length, "text/plain");
                }

            }
            catch (MinioException e)
            {
                throw e;
            }

            return null;
        }

        /// <summary>
        /// 新增未讀訊息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async void AddUserMsg(List<PostQueue> orders)
        {
            string targetBucketStr = "usermessage";            

            List<UserAlert> Result = orders
                .GroupBy(x => x.orderUser)
                .Select(y =>
                {
                    var result = new UserAlert
                    {
                        Account = y.First().orderUser
                    };

                    result.Content = y.Select(x => new UserAlert.AlertContent
                    {
                        AlertTime = DateTime.Now.ToString(),
                        OrderId = x.orderId,
                        StateMsg = x.statMsg
                    }).ToList();

                    return result;

                }).ToList();


            //編碼轉換
            JsonSerializerOptions jso = new JsonSerializerOptions()
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            Result.ForEach(async x =>
            {
                try
                {
                    await MINIO.StatObjectAsync(targetBucketStr, x.Account + ".txt");
                    //物件存在
                    var fileName = x.Account + ".txt";
                    var memoryStream = new MemoryStream();
                    string objContent;

                    await MINIO.GetObjectAsync(targetBucketStr, fileName, (stream) =>
                    {
                        stream.CopyToAsync(memoryStream);
                    });

                    memoryStream.Position = 0;

                    objContent = System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());

                    UserAlert userAlerts = new UserAlert
                    {
                        Content = JsonSerializer.Deserialize<List<AlertContent>>(objContent)
                    };

                    userAlerts.Content.AddRange(x.Content);

                    var content = JsonSerializer.Serialize(userAlerts.Content, jso);
                    var stream = new MemoryStream();

                    using var writer = new StreamWriter(stream);
                    writer.Write(content);
                    writer.Flush();
                    stream.Position = 0;
                    await MINIO.PutObjectAsync(targetBucketStr, fileName, stream, stream.Length, "text/plain");
                }
                catch (MinioException e)
                {
                    //物件不存在
                    var stream = new MemoryStream();
                    using var writer = new StreamWriter(stream);
                    writer.Write(JsonSerializer.Serialize(x.Content, jso));
                    writer.Flush();
                    stream.Position = 0;
                    await MINIO.PutObjectAsync(targetBucketStr, @$"{x.Account}.txt", stream, stream.Length, "text/plain");
                }
            });           
        }

        public class UserAlert
        {
            public UserAlert()
            {
                Content = new List<AlertContent>();
            }

            public string Account { get; set; }
            public List<AlertContent> Content { get; set; }

            public class AlertContent
            {
                public string AlertTime { get; set; }
                public string OrderId { get; set; }
                public string StateMsg { get; set; }
            }
        }
    }
}
