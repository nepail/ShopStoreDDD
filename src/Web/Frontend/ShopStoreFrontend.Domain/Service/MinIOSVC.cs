#region 功能與歷史修改描述

/*
    描述:檔案儲存服務
    日期:2022-01-25    
    
 */

#endregion

using Microsoft.AspNetCore.Http;
using Minio;
using Minio.Exceptions;
using System.IO;
using System.Threading.Tasks;

namespace ShopStoreFrontend.Domain.Service
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
        /// 讀取usermessage上的未讀訊息
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public async Task<string> ReadUserMsg(string account)
        {
            try
            {
                //檢查物件是否存在
                var result = await MINIO.StatObjectAsync("usermessage", account + ".txt");

                var memoryStream = new MemoryStream();

                await MINIO.GetObjectAsync("usermessage", account + ".txt", (stream) =>
                  {
                      stream.CopyTo(memoryStream);
                  });

                memoryStream.Position = 0;
                await MINIO.RemoveObjectAsync("usermessage", account + ".txt");
                return System.Text.Encoding.UTF8.GetString(memoryStream.ToArray());
            }
            catch (MinioException e)
            {
                return null;                
            }
        }
    }
}
