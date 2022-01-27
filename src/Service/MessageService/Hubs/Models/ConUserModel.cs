#region 功能與歷史修改描述

/*
    描述:連線 User Model
    建立日期:2021-01-18

    描述:程式碼風格調整
    修改日期:2022-01-20

 */

#endregion

using System;

namespace MessageService.Hubs.Models
{
    public class ConUserModel
    {
        /// <summary>
        /// 連線ID
        /// </summary>
        /// <value></value>
        public string ConnectionID { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        /// <value></value>
        public string UserName { get; set; }
        /// <summary>
        /// 帳號
        /// </summary>
        /// <value></value>
        public string UserAccount { get; set; }
        /// <summary>
        /// 上線時間
        /// </summary>
        /// <value></value>
        public DateTime OnlineTime { get; set; }
    }
}
