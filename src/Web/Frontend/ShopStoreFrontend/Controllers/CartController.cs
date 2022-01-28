#region 功能與歷史修改描述

/*
    描述:購物車控制器
    日期:2021-11-29

    描述:程式碼風格調整
    日期:2022-01-10

 */

#endregion

using Microsoft.AspNetCore.Mvc;
using NLog;
using ShopStoreFrontend.Domain.Models;
using ShopStoreFrontend.Domain.Models.Interface;
using ShopStoreFrontend.Domain.Service;
using ShopStoreFrontend.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ShopStore.Controllers
{
    public class CartController : Controller
    {
        private readonly static Logger LOGGER = LogManager.GetCurrentClassLogger();
        private readonly ICart CART;

        public CartController(ICart cart)
        {
            CART = cart;
        }

        public IActionResult Index()
        {
            //向 Session 取得商品列表
            List<CartItem> CartItems = SessionSVC.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");


            //計算商品總額
            if (CartItems != null)
            {
                //檢查商品
                CartItems = CART.CheckCartItem(CartItems);
                ViewBag.Total = CartItems.Sum(m => m.SubTotal);
            }
            else
            {
                ViewBag.Total = 0;
            }

            return View("/Views/Frontend/Cart/Index.cshtml", CartItems);
        }

        /// <summary>
        /// 單筆新增購物車
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult AddtoCart(string id)
        {
            UpdateCart(id, null);

            return NoContent(); // HttpStatus 204: 請求成功但不更新畫面
        }

        /// <summary>
        /// 願望清單新增購物車
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public IActionResult AddListToCart(string[] list)
        {
            int addedItem = UpdateCart(null, list);

            return Json(new { success = true, message = "新增購物車成功", addedItem });
        }

        /// <summary>
        /// 更新購物車
        /// </summary>
        private int UpdateCart(string id, string[] idList)
        {
            if (id != null)
            {
                ProductsViewModel cartItem = CART.Single(id);

                //取得商品資料
                CartItem item = new CartItem
                {
                    Product = cartItem,
                    Amount = 1,
                    SubTotal = cartItem.f_price
                };

                //判斷 Session 內有無購物車
                if (SessionSVC.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart") == null)
                {
                    //如果沒有已存在購物車: 建立新的購物車
                    List<CartItem> cart = new List<CartItem>
                    {
                        item
                    };
                    SessionSVC.SetObjectAsJson(HttpContext.Session, "cart", cart);
                }
                else
                {
                    //如果已存在購物車: 檢查有無相同的商品，有的話只調整數量
                    List<CartItem> cart = SessionSVC.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

                    int index = cart.FindIndex(m => m.Product.f_id.Equals(int.Parse(id)));

                    if (index != -1)
                    {
                        cart[index].Amount += item.Amount;
                        cart[index].SubTotal += item.SubTotal;
                    }
                    else
                    {
                        cart.Add(item);
                    }

                    SessionSVC.SetObjectAsJson(HttpContext.Session, "cart", cart);
                }                
            }

            if (idList != null)
            {
                List<ProductsViewModel> cartItems = CART.QueryMutiple(idList);

                List<CartItem> cartItemsOfList = (from a in cartItems
                                                  select new CartItem
                                                  {
                                                      Product = a,
                                                      Amount = 1,
                                                      SubTotal = a.f_price
                                                  }).ToList();

                //判斷 Session 內有無購物車
                if (SessionSVC.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart") == null)
                {
                    SessionSVC.SetObjectAsJson(HttpContext.Session, "cart", cartItemsOfList);
                }
                else
                {
                    //如果已存在購物車: 檢查有無相同的商品，有的話只調整數量
                    List<CartItem> cart = SessionSVC.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

                    foreach (CartItem item in cartItemsOfList)
                    {
                        int index = cart.FindIndex(m => m.Product.f_id.Equals(item.Product.f_id));

                        if (index != -1)
                        {
                            cart[index].Amount += item.Amount;
                            cart[index].SubTotal += item.SubTotal;
                        }
                        else
                        {
                            cart.Add(item);
                        }

                        SessionSVC.SetObjectAsJson(HttpContext.Session, "cart", cart);
                    }
                }

                return cartItemsOfList.Count;
            }

            return 0;
        }

        /// <summary>
        /// 移除購物車Item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult RemoveItem(string id)
        {
            //向 Session 取得商品列表
            List<CartItem> cart = SessionSVC.GetObjectFromJson<List<CartItem>>(HttpContext.Session, "cart");

            //用FindIndex查詢目標在List裡的位置
            int index = cart.FindIndex(m => m.Product.f_id.Equals(int.Parse(id)));
            cart.RemoveAt(index);

            if (cart.Count < 1)
            {
                SessionSVC.Remove(HttpContext.Session, "cart");
            }
            else
            {
                SessionSVC.SetObjectAsJson(HttpContext.Session, "cart", cart);
            }

            return Json(new { success = true, message = $"刪除產品 {id} 成功" });
        }

        /// <summary>
        /// 建立新的訂單
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult CreateNewOrder([FromBody] List<OrderItem> data)
        {
            //清空購物車
            HttpContext.Session.Clear();

            OrderModel orderModel = new OrderModel()
            {
                f_id = DateTime.Now.ToString("yyyyMMddHHmmss"),
                f_memberid = User.FindFirst(ClaimTypes.NameIdentifier).Value,
                f_orderTime = DateTime.Now,
                f_status = 1,
                f_shippingMethod = 1,
                Orderlist = data
            };

            try
            {
                var result = CART.InsertOrderItem(orderModel);

                if (result <= 0) return Json(new { success = false, message = "訂單建立失敗" });

                return Json(new { success = true, message = $"成功新增 {result} 筆訂單" });
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return Json(new { success = false, message = "訂單新增失敗" });
            }
        }
    }
}
