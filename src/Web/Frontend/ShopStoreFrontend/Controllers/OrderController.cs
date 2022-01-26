#region 功能與歷史修改描述

/*
    描述:訂單管理
    建立日期:2021-11-29

    描述:程式碼風格調整
    修改日期:2022-01-10

 */

#endregion

using Microsoft.AspNetCore.Mvc;
using NLog;
using ShopStoreFrontend.Domain.Models.Interface;
using ShopStoreFrontend.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace ShopStore.Controllers
{
    public class OrderController : Controller
    {
        private static Logger LOGGER = LogManager.GetCurrentClassLogger();
        private readonly IOrders ORDERS;

        public OrderController(IOrders orders)
        {
            ORDERS = orders;
        }

        /// <summary>
        /// 我的訂單
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            List<OrderViewModel> model;
            string userId;

            try
            {
                var user = User.Identity.Name;                
                userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
                model = ORDERS.GetOrderList(userId);
                return View("/Views/Frontend/Order/Index.cshtml", model);
            }
            catch (Exception ex)
            {
                LOGGER.Error(ex);
                return RedirectToAction("/Home/Error");
            }
        }

        /// <summary>
        /// 取消訂單
        /// </summary>
        /// <param name="ordernum"></param>
        /// <returns></returns>
        public IActionResult CancelOrder(string ordernum)
        {

            try
            {
                ORDERS.DelOrder(ordernum);
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
            }

            return Json(new { success = true, message = "successfull" });
        }
    }
}
