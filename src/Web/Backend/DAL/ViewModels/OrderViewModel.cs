using System.Collections.Generic;

namespace ShopStore.ViewModels
{
    public class OrderViewModel
    {
        /// <summary>
        /// 訂單編號
        /// </summary>
        public string Num { get; set; }

        /// <summary>
        /// 訂單日期
        /// </summary>
        public string Date { get; set; }

        /// <summary>
        /// 配送狀態
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 配送的CSS Style
        /// </summary>
        public string StatusBadge { get; set; }
        
        /// <summary>
        /// 運送方式
        /// </summary>
        public string ShippingMethod { get; set; }

        /// <summary>
        /// 運送的CSS Style
        /// </summary>
        public string ShippingBadge { get; set; }

        /// <summary>
        /// 訂單金額
        /// </summary>
        public int TotalAmountOfMoney { get; set; }

        /// <summary>
        /// 訂購總數量
        /// </summary>
        public int TotalAmountOfProducts { get; set; }

        /// <summary>
        /// 訂單明細
        /// </summary>
        public List<ItemDetail> ListOfItem { get; set; }

    }

    public class ItemDetail
    {
        public string Id { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 數量
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 小計
        /// </summary>
        public int AmountOfMoney { get; set; }
    }
}
