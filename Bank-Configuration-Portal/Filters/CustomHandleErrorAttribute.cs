using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Resources;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Bank_Configuration_Portal.Filters
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            var ex = filterContext.Exception;
            var http = filterContext.HttpContext;

            Logger.LogError(ex);

            bool isAntiForgeryError =
                ex is HttpAntiForgeryException ||
                ex.InnerException is HttpAntiForgeryException ||
                (ex.Message ?? "").IndexOf("anti-forgery", StringComparison.OrdinalIgnoreCase) >= 0 ||
                (ex.InnerException?.Message ?? "").IndexOf("anti-forgery", StringComparison.OrdinalIgnoreCase) >= 0;

            if (isAntiForgeryError)
            {
                filterContext.ExceptionHandled = true;

                if (http.Request.IsAjaxRequest())
                {
                    filterContext.Result = new JsonResult
                    {
                        Data = new
                        {
                            ok = false,
                            message = "Your session changed or expired. Please reload and try again."
                        },
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet
                    };
                    http.Response.StatusCode = 400;
                    http.Response.TrySkipIisCustomErrors = true;
                    return;
                }

                var isAuth = http.User?.Identity?.IsAuthenticated == true;

                if (isAuth)
                {
                    SetTempData(filterContext, "Info", Language.Already_Signed_In);
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        { "controller", "Branch" },
                        { "action", "Index" }
                    });
                }
                else
                {
                    SetTempData(filterContext, "Error", Language.Session_Expired);
                    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                    {
                        { "controller", "Login" },
                        { "action", "Index" }
                    });
                }

                return;
            }

            filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
            {
                { "controller", "Error" },
                { "action", "Generic" },
                { "errorMessage", "An unexpected error occurred. Please contact your system administrator." }
            });

            filterContext.ExceptionHandled = true;
        }

        private static void SetTempData(ExceptionContext ctx, string key, string message)
        {
            try
            {
                ctx.Controller.TempData[key] = message;
            }
            catch {}
        }
    }
}
