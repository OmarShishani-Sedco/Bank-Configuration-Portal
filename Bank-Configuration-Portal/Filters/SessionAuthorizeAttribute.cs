using System;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace Bank_Configuration_Portal.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var ctx = HttpContext.Current;
            var req = ctx?.Request;
            var session = ctx?.Session;

            var path = (req?.Path ?? string.Empty).ToLowerInvariant();

            bool isLoginIndex =
                string.IsNullOrEmpty(path) || path == "/" ||
                path == "/login" ||
                path.StartsWith("/login/index");

            bool isChangePassword =
                path.StartsWith("/login/changepassword");

            if (isLoginIndex && IsLoggedIn(session))
            {
                var mustChange = (bool?)session["MustChangePassword"] ?? false;
                filterContext.Result = new RedirectResult(
                    mustChange ? "~/Login/ChangePassword" : "~/Branch/Index"
                );
                return;
            }

            if (isChangePassword)
            {
                if (!IsLoggedIn(session))
                {
                    filterContext.Result = new RedirectResult("~/Login/Index");
                    return;
                }

                var mustChange = (bool?)session["MustChangePassword"] ?? false;
                if (!mustChange)
                {
                    // already changed; don't allow revisiting
                    filterContext.Result = new RedirectResult("~/Branch/Index");
                    return;
                }
            }

            var skipAuthorization =
                filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);

            if (skipAuthorization)
                return;

            if (!IsLoggedIn(session))
            {
                filterContext.Result = new RedirectResult("~/Login/Index");
                return;
            }

            var must = (bool?)session["MustChangePassword"] ?? false;
            if (must && !IsAllowedWhileMustChange(path))
            {
                filterContext.Result = new RedirectResult("~/Login/ChangePassword");
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        private static bool IsLoggedIn(HttpSessionState session)
            => session?["BankId"] != null && session?["UserName"] != null;

        private static bool IsAllowedWhileMustChange(string lowerPath)
            => lowerPath.StartsWith("/login/changepassword")
               || lowerPath.StartsWith("/login/logout")
               || lowerPath.StartsWith("/maintenance/");
    }
}
