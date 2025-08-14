using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal.Filters
{
    public class ClaimsAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var req = HttpContext.Current?.Request;
            var path = (req?.Path ?? string.Empty).ToLowerInvariant();

            var user = filterContext.HttpContext.User as ClaimsPrincipal;
            bool isAuth = user?.Identity?.IsAuthenticated == true;
            string bankId = user?.FindFirst("BankId")?.Value;
            bool mustChange = user?.FindFirst("MustChangePassword")?.Value == "true";

            bool isLoginIndex =
                string.IsNullOrEmpty(path) || path == "/" ||
                path == "/login" || path.StartsWith("/login/index");

            bool isChangePassword = path.StartsWith("/login/changepassword");

            // Never show the login page to authenticated users
            if (isLoginIndex && isAuth)
            {
                filterContext.Result = new RedirectResult(mustChange ? "~/Login/ChangePassword" : "~/Branch/Index");
                return;
            }

            // Gate /Login/ChangePassword strictly
            if (isChangePassword)
            {
                if (!isAuth)
                {
                    filterContext.Result = new RedirectResult("~/Login/Index");
                    return;
                }
                if (!mustChange)
                {
                    filterContext.Result = new RedirectResult("~/Branch/Index");
                    return;
                }
            }

            // Respect [AllowAnonymous] after the explicit gates above
            var skipAuthorization =
                filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true) ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);
            if (skipAuthorization) return;

            // Require auth everywhere else
            if (!isAuth || string.IsNullOrEmpty(bankId))
            {
                filterContext.Result = new RedirectResult("~/Login/Index");
                return;
            }

            // Enforce must-change window
            if (mustChange && !IsAllowedWhileMustChange(path))
            {
                filterContext.Result = new RedirectResult("~/Login/ChangePassword");
                return;
            }

            base.OnActionExecuting(filterContext);
        }

        private static bool IsAllowedWhileMustChange(string lowerPath) =>
            lowerPath.StartsWith("/login/changepassword") ||
            lowerPath.StartsWith("/login/logout") ||
            lowerPath.StartsWith("/maintenance/");
    }
}
