#region 功能與歷史修改描述

/*
    描述:驗證表單
    建立日期:2021-11-29

    描述:程式碼風格調整
    修改日期:2022-01-10

 */

#endregion

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopStoreFrontend.Domain.Models.Interface;
using System.Threading.Tasks;

namespace ShopStore.Controllers
{
    [AllowAnonymous]
    public class VerifyController : Controller
    {
        private readonly IMembers MEMBERS;

        public VerifyController(IMembers members)
        {
            MEMBERS = members;
        }
        
        /// <summary>
        /// 檢查 Email 是否重複
        /// </summary>
        /// <param name="f_mail"></param>
        /// <returns></returns>
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyEmail(string f_mail)
        {
            if (await MEMBERS.VerifyEmailAsync(f_mail))
            {
                return Json($"{f_mail} 已經使用過，請使用其他 Email 註冊");
            }
            return Json(true);
        }

        /// <summary>
        /// 檢查 Account 是否重複
        /// </summary>
        /// <param name="f_mail"></param>
        /// <returns></returns>
        [AcceptVerbs("GET", "POST")]
        public async Task<IActionResult> VerifyAccount(string f_account)
        {
            if (await MEMBERS.VerifyAccountAsync(f_account))
            {
                return Json($"{f_account} 已經使用過，請使用其他帳號註冊");
            }
            return Json(true);
        }
    }
}
