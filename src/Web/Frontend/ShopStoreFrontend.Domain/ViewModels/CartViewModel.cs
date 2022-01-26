namespace ShopStoreFrontend.Domain.ViewModels
{
    /// <summary>
    /// 購物車 ViewModel
    /// </summary>
    public class CartViewModel
    {
        public int Id { get; set; }        
        public int IsOpen { get; set; }
        public int ProductId { get; set; }  //商品ID
        public int Amount { get; set; }     //數量
        public int SubTotal { get; set; }   //小計
    }
    public class CartItem : CartViewModel
    {
        public ProductsViewModel Product { get; set; } //商品內容   
    }
}
