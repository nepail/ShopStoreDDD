using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace ShopStoreFrontend.Filters
{
    public class ActionFilter : Attribute, IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext context)
        {

        }

        public void OnActionExecuting(ActionExecutingContext context)
        {

        }
    }
}
