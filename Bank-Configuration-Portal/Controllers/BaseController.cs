using Bank_Configuration_Portal.Models;
using Bank_Configuration_Portal.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal.Controllers
{
    public class BaseController : Controller
    {
        protected ClaimsPrincipal CurrentUserClaims { get; private set; }
        protected BaseViewModel CurrentBaseModel { get; private set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);

            var cp = User as ClaimsPrincipal;
            CurrentUserClaims = cp;

            var baseModel = new BaseViewModel
            {
                IsLoggedIn = cp?.Identity?.IsAuthenticated == true,
                MustChangePassword = cp?.FindFirst("MustChangePassword")?.Value == "true",
                UserName = cp?.Identity?.Name,
                bankName = cp?.FindFirst("BankName")?.Value,
                TargetUrl = (cp?.Identity?.IsAuthenticated == true &&
                             cp?.FindFirst("MustChangePassword")?.Value != "true")
                            ? Url.Action("Index", "Branch")
                            : Url.Action("ChangePassword", "Login")
            };

            CurrentBaseModel = baseModel;
            ViewBag.BaseViewModel = baseModel;
        }

        [AllowAnonymous]
        public ActionResult ChangeLanguage(string lang, string returnUrl)
        {
            if (!string.IsNullOrEmpty(lang))
            {
                HttpCookie cookie = new HttpCookie("culture", lang)
                {
                    Expires = DateTime.Now.AddYears(1)
                };
                Response.Cookies.Add(cookie);
            }

            return Redirect(returnUrl ?? "/");
        }
        protected bool TryGetBankId(out int bankId)
        {
            bankId = 0;
            var cp = User as ClaimsPrincipal;
            var bankIdStr = cp?.FindFirst("BankId")?.Value;
            return int.TryParse(bankIdStr, out bankId);
        }

        protected ActionResult BankIdMissingRedirect()
        {
            TempData["Error"] = Language.Generic_Error;
            return RedirectToAction("Index", "Login");
        }

    }
}