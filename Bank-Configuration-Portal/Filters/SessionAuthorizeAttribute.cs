using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal.Filters
{
    public class SessionAuthorizeAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true)
                                  || filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), true);

            if (skipAuthorization)
            {
                return;
            }

            var bankId = HttpContext.Current.Session["BankId"];
            var username = HttpContext.Current.Session["UserName"];

            if (bankId == null || username == null)
            {
                filterContext.Result = new RedirectResult("~/Login/Index");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}