using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Models;
using Bank_Configuration_Portal.Models.Models;
using Bank_Configuration_Portal.Resources;
using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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

                if (!(await _bankManager.IsUserMappedToBankAsync(model.UserName, bank.Id)))
                {
                    ModelState.AddModelError("", Language.Login_Unauthorized_User);
                    return View(model);
                }


                var (valid, mustChange) = await _userManager.ValidateCredentialsAsync(model.UserName, model.Password);
                if (!valid)
                {
                    ModelState.AddModelError("", Language.Login_Invalid_Credentials);
                    return View(model);
                }

                FormsAuthentication.SetAuthCookie(model.UserName, false);

                Session["BankId"] = bank.Id;
                Session["UserName"] = model.UserName;
                Session["BankName"] = bank.Name;
                Session["MustChangePassword"] = mustChange;

                if (mustChange)
                    return RedirectToAction("ChangePassword");

                return RedirectToAction("Index", "Branch");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                ModelState.AddModelError("", Language.Generic_Error);
                return View(model);
            }
        }

        [HttpGet]
        public ActionResult ChangePassword()
        {
            if (Session["UserName"] == null)
                return RedirectToAction("Index");

            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {

            if (Session["UserName"] == null)
                return RedirectToAction("Index");

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var userName = (string)Session["UserName"];
                var ok = await _userManager.ChangePasswordAsync(userName, model.OldPassword, model.NewPassword);
                if (!ok)
                {
                    ModelState.AddModelError("", Language.ChangePassword_InvalidOldPassword);
                    return View(model);
                }
                Session["MustChangePassword"] = false;
                TempData["Success"] = Language.ChangePassword_Success;
                return RedirectToAction("Index", "Branch");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                ModelState.AddModelError("", Language.Generic_Error);
                return View(model);
            }
        }

        [AllowAnonymous]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            Session.Clear();
            Session.Abandon();


            return RedirectToAction("Index", "Login");
        }
    }
}
