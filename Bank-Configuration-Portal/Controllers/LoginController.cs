using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Models;
using Bank_Configuration_Portal.Resources;
using Microsoft.Owin.Security;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IBankManager _bankManager;
        private readonly IUserManager _userManager;

        public LoginController(IBankManager bankManager, IUserManager userManager)
        {
            _bankManager = bankManager;
            _userManager = userManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var bank = await _bankManager.GetByNameAsync(model.BankName);
                if (bank == null)
                {
                    ModelState.AddModelError("", Language.Login_Invalid_Bank);
                    return View(model);
                }

                if (!await _bankManager.IsUserMappedToBankAsync(model.UserName, bank.Id))
                {
                    ModelState.AddModelError("", Language.Login_Unauthorized_User);
                    return View(model);
                }

                var (valid, mustChange) = await _userManager.ValidateCredentialsAsync(model.UserName, model.Password);
                if (!valid && mustChange)
                {
                    ModelState.AddModelError("", Language.Inactive_User);
                    return View(model);
                }
                if (!valid)
                {
                    ModelState.AddModelError("", Language.Login_Invalid_Credentials);
                    return View(model);
                }

                var stamp = Startup.GetOrIssueNewStamp(model.UserName.ToLower(), bank.Id.ToString());

                // grab current UA/IP
                var ua = HttpContext.Request.UserAgent ?? "";
                var ip = HttpContext.Request.UserHostAddress ?? "";

                // OWIN cookie sign-in with claims
                var identity = new ClaimsIdentity("AppCookie");
                identity.AddClaim(new Claim(ClaimTypes.Name, model.UserName.ToLower()));
                identity.AddClaim(new Claim("BankId", bank.Id.ToString()));
                identity.AddClaim(new Claim("BankName", bank.Name));
                identity.AddClaim(new Claim("MustChangePassword", mustChange ? "true" : "false"));
                identity.AddClaim(new Claim("SessionStamp", stamp));
                identity.AddClaim(new Claim("UA", ua));
                identity.AddClaim(new Claim("IP", ip));

                HttpContext.GetOwinContext().Authentication.SignIn(
                    new AuthenticationProperties { IsPersistent = false }, identity);

                if (mustChange)
                    return RedirectToAction("ChangePassword");

                return RedirectToAction("Index", "Branch");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "LoginController.Index");
                ModelState.AddModelError("", Language.Generic_Error);
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var cp = CurrentUserClaims;
                var userName = CurrentBaseModel.UserName;

                var ok = await _userManager.ChangePasswordAsync(userName, model.OldPassword, model.NewPassword);
                if (!ok)
                {
                    ModelState.AddModelError("", Language.ChangePassword_InvalidOldPassword);
                    return View(model);
                }

                // Re-issue cookie with MustChangePassword=false
                var bankId = cp.FindFirst("BankId")?.Value ?? "";
                var bankName = cp.FindFirst("BankName")?.Value ?? "";

                var newIdentity = new ClaimsIdentity("AppCookie");
                newIdentity.AddClaim(new Claim(ClaimTypes.Name, userName));
                if (!string.IsNullOrEmpty(bankId))
                    newIdentity.AddClaim(new Claim("BankId", bankId));
                if (!string.IsNullOrEmpty(bankName)) 
                    newIdentity.AddClaim(new Claim("BankName", bankName));
                newIdentity.AddClaim(new Claim("MustChangePassword", "false"));


                var auth = HttpContext.GetOwinContext().Authentication;
                auth.SignOut("AppCookie");
                auth.SignIn(new AuthenticationProperties { IsPersistent = false }, newIdentity);

                TempData["Success"] = Language.ChangePassword_Success;
                return RedirectToAction("Index", "Branch");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "LoginController.ChangePassword(POST)");
                ModelState.AddModelError("", Language.Generic_Error);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            HttpContext.GetOwinContext().Authentication.SignOut("AppCookie");
            return RedirectToAction("Index", "Login");
        }
    }
}
