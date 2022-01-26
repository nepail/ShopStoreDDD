namespace ShopStoreBackend.Domain.Models.Manager.ViewModels.Product
{
    /// <summary>
    /// 庫存量檢查
    /// </summary>
    public class InventoryViewModel
    {
        /// <summary>
        /// 產品Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 庫存量
        /// </summary>
        public int Stock { get; set; }
    }
}
