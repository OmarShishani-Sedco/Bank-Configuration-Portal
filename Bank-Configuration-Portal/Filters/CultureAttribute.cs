using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal.Filters
{
    public class CultureAttribute : ActionFilterAttribute
    {
        private readonly string _defaultCulture = "en";

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var cultureCookie = filterContext.HttpContext.Request.Cookies["culture"];
            var cultureName = cultureCookie != null ? cultureCookie.Value : _defaultCulture;

            try
            {
                var culture = CultureInfo.CreateSpecificCulture(cultureName);
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
            }
            catch (CultureNotFoundException)
            {
                Thread.CurrentThread.CurrentCulture = new CultureInfo(_defaultCulture);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(_defaultCulture);
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
