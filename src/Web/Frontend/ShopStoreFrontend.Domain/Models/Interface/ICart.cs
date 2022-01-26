
using ShopStoreFrontend.Domain.ViewModels;
using System.Collections.Generic;

namespace ShopStoreFrontend.Domain.Models.Interface
{
    /// <summary>
    /// 購物車 Interface
    /// </summary>
    public interface ICart
    {
        /// <summary>
        /// 取得產品資訊
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProductsViewModel Single(string id);
        /// <summary>
        /// 將訂單寫入資料庫
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertOrderItem(OrderModel model);
        /// <summary>
        /// 取多筆產品資訊
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<ProductsViewModel> QueryMutiple(string[] list);
        /// <summary>
        /// 檢查購物車商品
        /// </summary>
        /// <param name="cartItems"></param>
        /// <returns></returns>
        public List<CartItem> CheckCartItem(List<CartItem> cartItems);
    }
}
