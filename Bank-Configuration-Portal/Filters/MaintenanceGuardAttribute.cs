using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public sealed class MaintenanceGuardAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = filterContext.HttpContext.Request;
            var secret = req["secret"];
            var expectedSecret = ConfigurationManager.AppSettings["MaintenanceSecret"];
            bool modeOn = bool.TryParse(ConfigurationManager.AppSettings["MaintenanceMode"], out var on) && on;

            if (!modeOn || string.IsNullOrEmpty(expectedSecret) || secret != expectedSecret)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new System.Web.Routing.RouteValueDictionary
                    {
                        { "controller", "Error" },
                        { "action", "Forbidden" }
                    });
                return;
            }


            base.OnActionExecuting(filterContext);
        }
    }
}