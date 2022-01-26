using System;
using System.ComponentModel.DataAnnotations;

namespace ShopStoreFrontend.Domain.Models
{
    public class ProductsModel
    {
        /// <summary>
        /// 產品編號
        /// </summary>
        public string pId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [Display(Name = "名稱"), Required(ErrorMessage = "請輸入商品名稱")]
        public string name { get; set; }

        /// <summary>
        /// 售價
        /// </summary>
        [Display(Name = "售價"), Required(ErrorMessage = "請輸入商品售價"),]
        public int price { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述"), Required(ErrorMessage = "請輸入商品描述")]
        public string description { get; set; }

        /// <summary>
        /// 分類
        /// </summary>
        [Display(Name = "類型"), Required(ErrorMessage = "請輸入商品分類")]
        public int categoryId { get; set; }

        /// <summary>
        /// 庫存數量
        /// </summary>
        [Display(Name = "庫存數量"), Required(ErrorMessage = "請輸入庫存量")]
        public int stock { get; set; } = 1;

        /// <summary>
        /// 是否刪除
        /// </summary>
        [Display(Name = "是否刪除")]
        public int isDel { get; set; } = 0;

        /// <summary>
        /// 是否開放
        /// </summary>
        [Display(Name = "是否開放"), Required(ErrorMessage = "請選擇是否開放")]
        public int isOpen { get; set; } = 1;

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime createTime { get; set; }

        /// <summary>
        /// 更新時間
        /// </summary>
        public DateTime updateTime { get; set; }
    }    
}
