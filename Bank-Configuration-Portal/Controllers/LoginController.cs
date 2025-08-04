using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Models;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Bank_Configuration_Portal.Resources;

namespace Bank_Configuration_Portal.Controllers
{
    public class LoginController : BaseController
    {
        private readonly IBankManager _bankManager;

        public LoginController(IBankManager bankManager)
        {
            _bankManager = bankManager;
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
        public ActionResult Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var bank = _bankManager.GetByName(model.BankName);
                if (bank == null)
                {
                    ModelState.AddModelError("", Language.Login_Invalid_Bank);
                    return View(model);
                }
                if (!_bankManager.IsUserMappedToBank(model.UserName, bank.Id))
                {
                    ModelState.AddModelError("", Language.Login_Unauthorized_User);
                    return View(model);
                }

                Session["BankId"] = bank.Id;
                Session["UserName"] = model.UserName;

                return RedirectToAction("Index", "Branch");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                ModelState.AddModelError("", Language.Generic_Error);
                return View(model);
            }
        }



        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
