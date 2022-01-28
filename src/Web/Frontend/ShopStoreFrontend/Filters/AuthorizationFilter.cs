#region 功能與歷史修改描述

/*
    描述:驗證用戶有效性，防止重複登入
    日期:2022-01-25

 */

#endregion

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Distributed;
using System.Threading.Tasks;

namespace ShopStoreFrontend.Filters
{
    public class AuthorizationFilter : IAsyncAuthorizationFilter
    {
        private readonly IDistributedCache CACHE;
        public AuthorizationFilter(IDistributedCache cache)
        {
            CACHE = cache;
        }

        /// <summary>
        /// 驗證用戶有效性，防止重複登入
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        async Task IAsyncAuthorizationFilter.OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                string userid = context.HttpContext.User.FindFirst("Account").Value;
                
                if (!context.HttpContext.Request.Cookies[userid].Equals(CACHE.GetString(userid)))
                {
                    string cookieType = context.HttpContext.User.Identity.AuthenticationType;

                    //藉由 AuthenticationType 判斷登入用戶
                    string controller = cookieType == "manager" ? cookieType : "Home";

                    //將使用者登出
                    await context.HttpContext.SignOutAsync(context.HttpContext.User.Identity.AuthenticationType);
                    //導至錯誤頁面
                    context.Result = new RedirectToRouteResult(
                        new RouteValueDictionary
                        {
                            {"controller", controller },
                            {"action", "Index" }
                        });
                }
            }            
        }
    }
}
