#region 功能與歷史修改描述

/*
    描述:產品資料庫
    建立日期:2021-11-24

    描述:程式碼風格調整
    修改日期:2022-01-07

 */

#endregion

using Dapper;
using NLog;
using ShopStoreFrontend.Domain.Models;
using ShopStoreFrontend.Domain.Models.Interface;
using ShopStoreFrontend.Domain.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using static ShopStoreFrontend.Domain.ViewModels.ProductsViewModel;

namespace ShopStoreFrontend.Persistence.Models.Service
{
    public class ProductsSVC : IProducts
    {
        private static readonly Logger LOGGER = LogManager.GetCurrentClassLogger();
        private readonly SqlConnection CONNECTION;
        public ProductsSVC(SqlConnection connection)
        {
            CONNECTION = connection;
        }

        /// <summary>
        /// 取得商品列表
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ProductsViewModel>> GetProductsAsync(int isopen)
        {
            try
            {
                using var conn = CONNECTION;
                var result = await conn.QueryAsync<ProductsViewModel>("pro_fr_getProducts", new { isopen }, commandType: System.Data.CommandType.StoredProcedure);
                return result;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                throw ex;
            }
        }

        /// <summary>
        /// 取得類別列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<CategoriesViewModel> GetCatgoryId()
        {
            try
            {
                using var conn = CONNECTION;
                var result = conn.Query<CategoriesViewModel>(@"SELECT 
                                                                 [f_id]                                                                
                                                                ,[f_name] FROM t_categories WITH(NOLOCK)");
                return result;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return null;
            }
        }

        /// <summary>
        /// 新增商品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public bool AddProducts(ProductsViewModel model)
        {            
            try
            {
                using var conn = CONNECTION;
                var productsModel = new ProductsModel
                {
                    pId = model.f_pId,
                    name = model.f_name,
                    price = model.f_price,                    
                    description = model.f_description,
                    categoryId = model.f_categoryId,
                    stock = model.f_stock,
                    isDel = model.f_isDel,
                    isOpen = model.f_isOpen,
                    updateTime = DateTime.Now,
                    createTime = DateTime.Now
                };

                return conn.Execute("pro_fr_addProduct", productsModel, commandType: System.Data.CommandType.StoredProcedure) > 0;
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return false;
            }                        
        }


        /// <summary>
        /// 更新商品
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<bool> EditProductById(ProductsViewModel model)
        {
            ProductModel productModel = new ProductModel()
            {
                f_pId = model.f_pId,
                f_name = model.f_name,
                f_price = model.f_price,                
                f_description = model.f_description,
                f_categoryId = model.f_categoryId,
                f_stock = model.f_stock,
                f_isDel = model.f_isDel,
                f_isOpen = model.f_isOpen,
                f_updateTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")                
            };

            try
            {
                using var conn = CONNECTION;
                return await conn.ExecuteAsync("pro_bg_editProduct", productModel, commandType: System.Data.CommandType.StoredProcedure) > 0;                
            }
            catch (Exception ex)
            {
                LOGGER.Debug(ex, "Debug");
                return false;
            }
        }
    }
}
