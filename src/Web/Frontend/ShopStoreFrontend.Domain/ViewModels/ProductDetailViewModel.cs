namespace ShopStoreFrontend.Domain.ViewModels
{
    public class ProductDetailViewModel
    {
        /// <summary>
        /// 流水號
        /// </summary>
        public string Id { get; set; }

        public string PId { get; set; }

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 售價
        /// </summary>
        public int Price { get; set; }

        /// <summary>
        /// 類型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 細項描述
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 圖片路徑
        /// </summary>
        public string ImgPath { get; set; }
    }
}
