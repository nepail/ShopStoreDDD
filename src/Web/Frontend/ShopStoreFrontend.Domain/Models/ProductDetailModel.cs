namespace ShopStoreFrontend.Domain.Models
{
    public class ProductDetailModel
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public string f_id { get; set; }

        /// <summary>
        /// 產品ID
        /// </summary>
        public string f_pId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string f_name { get; set; }

        /// <summary>
        /// 價格
        /// </summary>
        public int f_price { get; set; }

        /// <summary>
        /// 類型
        /// </summary>
        public int f_categoryId { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string f_description { get; set; }

        /// <summary>
        /// 內容
        /// </summary>
        public string f_content { get; set; }

        /// <summary>
        /// 圖片路徑
        /// </summary>
        public string f_picName { get; set; }
    }
}
