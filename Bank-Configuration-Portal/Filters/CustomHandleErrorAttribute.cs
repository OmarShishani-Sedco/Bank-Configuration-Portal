using System;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Bank_Configuration_Portal.Common;
namespace Bank_Configuration_Portal.Filters
{
    public class CustomHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.ExceptionHandled)
                return;

            Exception ex = filterContext.Exception;
            Logger.LogError(ex);

            bool isAntiForgeryError = ex is HttpAntiForgeryException ||
                                      (ex.Message != null && ex.Message.Contains("required anti-forgery cookie"));

            if (isAntiForgeryError)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                    { "controller", "Error" },
                    { "action", "Generic" },
                    { "errorMessage", "Your session has expired or the security token is invalid. Please try again." }
                    });

                FormsAuthentication.SignOut();
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary
                    {
                    { "controller", "Error" },
                    { "action", "Generic" },
                    { "errorMessage", "An unexpected error occurred. Please contact your system administrator." }
                    });
            }

            filterContext.ExceptionHandled = true;
        }
    }
}