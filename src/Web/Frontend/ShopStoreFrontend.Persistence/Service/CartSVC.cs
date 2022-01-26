#region 功能與歷史修改描述

/*
    描述:購物車資料庫處理
    建立日期:2021-11-24

    描述:程式碼風格挑整&移除未使用的code
    修改日期:2022-01-07

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
    public class CartSVC : ICart
    {
        private readonly SqlConnection CONNECTION;
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        public CartSVC(SqlConnection connection)
        {
            CONNECTION = connection;
        }

        /// <summary>
        /// 檢查購物車商品
        /// </summary>
        /// <param name="cartItems"></param>
        /// <returns></returns>
        public List<CartItem> CheckCartItem(List<CartItem> cartItems)
        {
            try
            {
                List<string> prolist = cartItems.Select(x => x.Product.f_id.ToString()).ToList();
                using var conn = CONNECTION;
                var strSql = @"SELECT * FROM t_products WHERE f_id in @f_id";
                List<ProductsViewModel> result = conn.Query<ProductsViewModel>(strSql, new { f_id = prolist }).ToList();

                List<CartItem> items = (
                                        from a in cartItems
                                        select new CartItem
                                        {
                                            Id = result.Where(x => x.f_id == a.Product.f_id).Single().f_id,
                                            Amount = a.Amount,
                                            SubTotal = result.Where(x => x.f_id == a.Product.f_id).Single().f_isOpen == 1 ? a.SubTotal : 0,
                                            Product = a.Product,
                                            IsOpen = result.Where(x => x.f_id == a.Product.f_id).Single().f_isOpen,
                                        }).ToList();

                return items;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return null;
            }
        }

        /// <summary>
        /// 取得產品資訊
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProductsViewModel Single(string id)
        {
            using var conn = CONNECTION;
            string strSql = @"select * from ShoppingDB.dbo.t_products where f_id = @f_id";
            return conn.QuerySingle<ProductsViewModel>(strSql, new { f_id = id });
        }

        /// <summary>
        /// 取多筆產品資訊
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<ProductsViewModel> QueryMutiple(string[] list)
        {
            List<string> lists = list.ToList();
            using var conn = CONNECTION;
            string strSql = @"select * from t_products where f_id in @f_id";
            return (List<ProductsViewModel>)conn.Query<ProductsViewModel>(strSql, new { f_id = lists });
        }

        /// <summary>
        /// 將訂單寫入資料庫
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertOrderItem(OrderModel model)
        {
            using TransactionScope scope = new TransactionScope();
            using var conn = CONNECTION;

            try
            {
                List<string> prolist = model.Orderlist.Select(itemid => itemid.f_productid).ToList();
                string strSql1 = @"select f_id, f_pid, f_price, f_isOpen from t_products where f_id in @f_ids";
                IEnumerable<ProductsViewModel> prolists = CONNECTION.Query<ProductsViewModel>(strSql1, new { f_ids = prolist });

                //總計金額不包含已下架商品
                model.f_total = prolists.Where(x => x.f_isOpen == 1).Sum(x => x.f_price);

                var itemRemove = prolists.Where(x => x.f_isOpen == 0);

                //將已下架商品從訂單明細移除
                foreach (var a in itemRemove)
                {
                    if (a.f_isOpen == 0)
                    {
                        model.Orderlist.RemoveAll(x => x.f_productid == a.f_id.ToString());
                    }
                }

                //將訂單編號寫入明細
                foreach (OrderItem a in model.Orderlist)
                {
                    a.f_orderId = model.f_id;
                }

                //訂單中沒有已上架商品
                if (model.Orderlist.Count <= 0) return 0;

                //寫入訂單主表
                string strSql = @"insert into t_orders(f_id
                                                      ,f_memberid
                                                      ,f_orderTime
                                                      ,f_status
                                                      ,f_shippingmethod
                                                      ,f_total
                                                      ,f_ispaid
                                                      ,f_isdel
                                                      ) values (
                                                       @f_id
                                                      ,@f_memberid
                                                      ,@f_orderTime
                                                      ,@f_status
                                                      ,@f_shippingmethod
                                                      ,@f_total
                                                      ,@f_ispaid
                                                      ,@f_isdel)";
                int result = conn.Execute(strSql, model);
                //將訂單明細寫入表
                string strDetailSql =
                @"insert into t_orderDetails(f_orderId, f_productid, f_amount, f_isdel) values (@f_orderId, @f_productid, @f_amount, @f_isdel)";
                conn.Execute(strDetailSql, model.Orderlist);

                //交易完成
                scope.Complete();
                return result;
            }
            catch (Exception ex)
            {
                //交易失敗，RollBack
                LOGGER.Debug(ex, "Debug");
                return 0;
            }
        }
    }
}
