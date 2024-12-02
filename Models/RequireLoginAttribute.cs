using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Chat.Models
{
    public class RequireLoginAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Kiểm tra nếu Session "UserId" không tồn tại
            if (filterContext.HttpContext.Session["UserId"] == null)
            {
                // Chuyển hướng đến trang Login
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                    { "controller", "Account" },
                    { "action", "Login" }
                    }
                );
            }

            base.OnActionExecuting(filterContext);
        }
    }
}