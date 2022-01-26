using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopStore.ViewModels
{
    /// <summary>
    /// 商品 ViewModel
    /// </summary>
    public class ProductsViewModel
    {        
        public ProductsViewModel()
        {
            SelectListItems = new List<SelectListItem>();

        }

        /// <summary>
        /// 流水號
        /// </summary>        
        public int f_id { get; set; }

        /// <summary>
        /// 產品編號
        /// </summary>
        public string f_pId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        [Display(Name = "名稱"), Required(ErrorMessage = "請輸入商品名稱")]
        public string f_name { get; set; }

        /// <summary>
        /// 售價
        /// </summary>
        [Display(Name = "售價"), Required(ErrorMessage = "請輸入商品售價"),]
        public int f_price { get; set; }

        /// <summary>
        /// 圖片名稱
        /// </summary>
        public string f_picName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        [Display(Name = "描述"), Required(ErrorMessage = "請輸入商品描述")]
        public string f_description { get; set; }

        /// <summary>
        /// 細項描述
        /// </summary>
        [Display(Name = "細項描述")]
        public string f_content { get; set; }

        /// <summary>
        /// 分類
        /// </summary>
        [Display(Name = "類型"), Required(ErrorMessage = "請輸入商品分類")]
        public int f_categoryId { get; set; }

        public string categoryName { get; set; }

        /// <summary>
        /// 庫存數量
        /// </summary>
        [Display(Name = "庫存數量"), Required(ErrorMessage = "請輸入庫存量")]
        public int f_stock { get; set; }

        /// <summary>
        /// 是否刪除
        /// </summary>
        [Display(Name = "是否刪除")]
        public int f_isDel { get; set; }

        /// <summary>
        /// 是否開放
        /// </summary>
        [Display(Name = "是否開放"), Required(ErrorMessage = "請選擇是否開放")]
        public int f_isOpen { get; set; }

        /// <summary>
        /// 圖片實體
        /// </summary>        
        [Display(Name = "圖片")]
        public IFormFile ProductPic { get; set; }

        public DateTime f_createTime { get; set; }

        public string CreateTime {
            get
            {
                return f_createTime.ToString("yyyy/MM/dd HH:mm:ss");
            }
            set
            {
            }
        }

        public string ContentText { get; set; }
        /// <summary>
        /// 類別清單
        /// </summary>
        public List<SelectListItem> SelectListItems { get; set; }

        public class ProductModel
        {
            public string f_pId { get; set; }
            public string f_name { get; set; }
            public int f_price { get; set; }            
            public string f_description { get; set; }
            public int f_categoryId { get; set; }
            public int f_stock { get; set; }
            public int f_isDel { get; set; }
            public int f_isOpen { get; set; }            
            public string f_updateTime { get; set; } = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");                        
        }
    }
}
