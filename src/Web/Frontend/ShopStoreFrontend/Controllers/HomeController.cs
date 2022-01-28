#region 功能與歷史修改描述

/*
    描述:首頁、註冊、願望清單
    日期:2021-11-29

    描述:程式碼風格調整&移除未使用功能
    日期:2022-01-10

 */

#endregion

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShopStoreFrontend.Domain.Models.Interface;
using ShopStoreFrontend.Domain.ViewModels;
using System.Security.Claims;

namespace ShopStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> LOGGER;
        private readonly IMembers MEMBERS;

        public HomeController(ILogger<HomeController> logger, IMembers members)
        {
            LOGGER = logger;
            MEMBERS = members;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View("/Views/Frontend/Home/Index.cshtml");
        }

        /// <summary>
        /// 帳號重複登入
        /// </summary>
        /// <returns></returns>        
        [Authorize(Roles = "Normal")]
        public IActionResult Error()
        {
            return View("/Views/Frontend/Home/Error.cshtml");
        }

        /// <summary>
        /// 會員中心
        /// </summary>
        /// <returns></returns>        
        [Authorize(Roles = "Normal")]
        public IActionResult UserProfile()
        {            
            var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));

            UserProfileViewModel userProfileViewModel = MEMBERS.GetMemberProfile(userId);

            return View("/Views/Frontend/Home/UserProfile.cshtml", userProfileViewModel);
        }

        /// <summary>
        /// 授權拒絕
        /// </summary>
        /// <returns></returns>                
        public IActionResult AccessDenied()
        {
            return View("/Views/Frontend/Home/AccessDenied.cshtml");
        }

        // [AllowAnonymous]
        // public IActionResult Privacy()
        // {
        //     return View();
        // }

        /// <summary>
        /// 願望清單
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public IActionResult WishList()
        {
            return View("/Views/Frontend/Home/WishList.cshtml");
        }
    }
}
