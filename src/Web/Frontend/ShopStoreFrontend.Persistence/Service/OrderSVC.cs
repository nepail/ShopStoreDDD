#region 功能與歷史修改描述

/*
    描述:前台訂單資料庫
    日期:2021-11-24

    描述:程式碼風格調整
    日期:2022-01-07

 */

#endregion

using Dapper;
using NLog;
using ShopStoreFrontend.Domain.Models;
using ShopStoreFrontend.Domain.Models.Interface;
using ShopStoreFrontend.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;

namespace ShopStoreFrontend.Persistence.Models.Service
{
    public class OrderSVC : IOrders
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        readonly private SqlConnection CONNECTION;
        public OrderSVC(SqlConnection connection)
        {
            CONNECTION = connection;
        }

        /// <summary>
        /// 取得訂單資料
        /// </summary>
        /// <param name="memberid">使用者ID</param>
        /// <returns></returns>
        public List<OrderViewModel> GetOrderList(string memberid)
        {
            using var conn = CONNECTION;
            string strSql = @"select * from t_orders where f_memberid = @f_memberid

                              select c.f_Id, c.f_orderId, c.f_productid, c.f_amount, p.f_name, p.f_price from t_orders o with (NOLOCK)
                              join t_orderDetails c with (NOLOCK) on o.f_id = c.f_orderId
                              join t_products p with (NOLOCK) on c.f_productid = p.f_id                              
                              where f_memberid = @f_memberid                              

                              select f_id, f_name, f_badge from t_orderStatus with (NOLOCK)
                              select f_id, f_name, f_badge from t_orderShipping with (NOLOCK)
";

            SqlMapper.GridReader result = conn.QueryMultiple(strSql, new { f_memberid = memberid });
            List<OrderModel> orderModels = result.Read<OrderModel>().ToList();
            List<OrderItem> orderdetail = result.Read<OrderItem>().ToList();
            List<OrderStatus> orderStatuses = result.Read<OrderStatus>().ToList();
            List<OrderShipping> orderShippings = result.Read<OrderShipping>().ToList();
            List<OrderViewModel> orderViewModels = (orderModels.Select(a => new OrderViewModel
            {
                Num = a.f_id,
                Date = a.f_orderTime.ToString(),
                Status = orderStatuses.SingleOrDefault(x => x.f_id == a.f_status)?.f_name,
                StatusBadge = orderStatuses.SingleOrDefault(x => x.f_id == a.f_status)?.f_badge,
                ShippingMethod = orderShippings.SingleOrDefault(x => x.f_id == a.f_shippingMethod)?.f_name,
                ShippingBadge = orderShippings.SingleOrDefault(x => x.f_id == a.f_shippingMethod)?.f_badge,
                TotalAmountOfMoney = a.f_total,
                TotalAmountOfProducts = orderdetail.Where(b => b.f_orderId == a.f_id).Sum(x => x.f_amount),
                ListOfItem = orderdetail.Where(b => b.f_orderId == a.f_id)
                                        .Select(x => new ItemDetail
                                        {
                                            Id = x.f_productid,
                                            Name = x.f_name,
                                            Amount = x.f_amount,
                                            Price = x.f_price,
                                            AmountOfMoney = orderdetail.First(item => item.f_productid == x.f_productid).f_price *
                                                            x.f_amount
                                        })
                                        .ToList()
            })).OrderByDescending(e => e.Date).ToList();


            return orderViewModels;
        }

        /// <summary>
        /// 取消訂單
        /// </summary>
        /// <param name="ordernum">訂單編號</param>
        /// <returns></returns>
        public bool DelOrder(string ordernum)
        {
            using TransactionScope scope = new TransactionScope();
            using var conn = CONNECTION;

            try
            {
                string strSql =
                    @"update t_orders set f_status = 5, f_isdel = 1 where f_id = @f_id
                      update t_orderDetails set f_isdel = 1 where f_orderId = @f_orderId";
                conn.Execute(strSql, new { f_id = ordernum, f_orderId = ordernum });
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return false;
            }
            
            scope.Complete();
            return true;
        }
    }
}
