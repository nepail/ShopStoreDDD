#region 功能與歷史修改描述

/*
    描述:訂單介面
    日期:2022-01-24

 */

#endregion

using ShopStoreFrontend.Domain.ViewModels;
using System.Collections.Generic;

namespace ShopStoreFrontend.Domain.Models.Interface
{
    /// <summary>
    /// 訂單 Interface
    /// </summary>
    public interface IOrders
    {
        /// <summary>
        /// 取得訂單資料
        /// </summary>
        /// <param name="memberid">使用者ID</param>
        /// <returns></returns>
        public List<OrderViewModel> GetOrderList(string memberid);
        /// <summary>
        /// 取消訂單
        /// </summary>
        /// <param name="ordernum">訂單編號</param>
        /// <returns></returns>
        public bool DelOrder(string ordernum);
    }
}
