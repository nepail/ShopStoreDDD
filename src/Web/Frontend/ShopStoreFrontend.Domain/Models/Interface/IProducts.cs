using ShopStoreFrontend.Domain.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ShopStoreFrontend.Domain.Models.Interface
{
    /// <summary>
    /// 產品 Interface
    /// </summary>
    public interface IProducts
    {
        /// <summary>
        /// 取得商品列表
        /// </summary>
        /// <returns></returns>
        Task <IEnumerable<ProductsViewModel>> GetProductsAsync(int isopen);

        /// <summary>
        /// 取得類別列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CategoriesViewModel> GetCatgoryId();

        /// <summary>
        /// 新增商品
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public bool AddProducts(ProductsViewModel request);

        /// <summary>
        /// 更新商品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<bool> EditProductById(ProductsViewModel model);
    }
}
