using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Filters;
using Bank_Configuration_Portal.Models;
using Bank_Configuration_Portal.Resources;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Bank_Configuration_Portal.Controllers
{
    [MaintenanceGuard]
    public class MaintenanceController : BaseController
    {
        private readonly IUserManager _userManager;
        private readonly IBankManager _bankManager;

        public MaintenanceController(IUserManager userManager, IBankManager bankManager)
        {
            _userManager = userManager;
            _bankManager = bankManager;
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult CreateOrResetUser(string secret)
        {
            var model = new MaintenanceViewModel { Secret = secret };

            // Show result if we just redirected from POST
            if (TempData["Maint_JustDid"] is bool b && b)
            {
                ViewBag.GeneratedPassword = TempData["Maint_Password"];
                ViewBag.Created = TempData["Maint_Created"];
                ViewBag.UserName = TempData["Maint_UserName"];
                ViewBag.BankId = TempData["Maint_BankId"];
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> CreateOrResetUser(MaintenanceViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!model.BankId.HasValue || !await _bankManager.BankExistsAsync(model.BankId.Value))
            {
                ModelState.AddModelError(nameof(model.BankId), Language.Login_Error_NoBank);
                return View(model);
            }

            try
            {
                var (pwd, created) = await _userManager.CreateOrResetUserAsync(model.UserName, model.BankId.Value);

                // Invalidate all current sessions for that user/bank
                Startup.RevokeStamp(model.UserName, model.BankId.Value.ToString());

                // Stash result for the GET to display once
                TempData["Maint_JustDid"] = true;
                TempData["Maint_Password"] = pwd;
                TempData["Maint_Created"] = created;
                TempData["Maint_UserName"] = model.UserName;
                TempData["Maint_BankId"] = model.BankId;

                return RedirectToAction("CreateOrResetUser", new { secret = model.Secret });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "MaintenanceController.CreateOrResetUser");
                ModelState.AddModelError("", Language.Generic_Error);
                return View(model);
            }
        }
    }
}
