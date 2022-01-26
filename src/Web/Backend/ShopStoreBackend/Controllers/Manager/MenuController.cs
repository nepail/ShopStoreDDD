#region 功能與歷史修改描述

/*
    描述:後台MENU
    建立日期:2021-12-03

    描述:程式碼風格調整
    修改日期:2022-01-10

 */

#endregion

using Microsoft.AspNetCore.Mvc;
using NLog;
using ShopStoreBackend.Domain.Models;
using ShopStoreBackend.Domain.Models.Interface;
using System;
using System.Threading.Tasks;

namespace ShopStore.Controllers.Manager
{
    public class MenuController : Controller
    {
        private readonly IManager MANAGER;
        private readonly static Logger LOGGER = LogManager.GetCurrentClassLogger();
        
        public MenuController(IManager manager)
        {
            MANAGER = manager;
        }

        /// <summary>
        /// 取得菜單
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetMenu(int userid)
        {
            //根據 user 的權限取得對應的菜單
            userid = 2;
            var result = MANAGER.GetMenu(userid);

            return Json(new { success = true, result = result });
        }

        /// <summary>
        /// 新增子菜單
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddSubMenu([FromBody] MenuViewModel model)
        {
            try
            {
                if(model != null)
                {
                    var result = await MANAGER.AddSubMenu(model);
                }
            }
            catch (Exception e)
            {
                LOGGER.Debug(e, "AddSubMenu Error");
            }

            return Json(new { success = true, message = "執行成功" });
        }


        /// <summary>
        /// 新增菜單
        /// </summary>
        /// <param name="menuModel"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult AddMenu(MenuModel menuModel)
        {
            var result = MANAGER.AddMenu(menuModel);

            return Json(new { success = true, message = "success" });
        }
    }
}
