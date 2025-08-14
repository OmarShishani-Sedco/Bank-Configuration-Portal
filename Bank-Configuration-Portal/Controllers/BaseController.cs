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
            TempData["Error"] = Resources.Language.Generic_Error;
            return RedirectToAction("Index", "Login");
        }

    }
}