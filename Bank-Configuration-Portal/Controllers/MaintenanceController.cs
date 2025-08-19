using Bank_Configuration_Portal.BLL.Interfaces;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Filters;
using Bank_Configuration_Portal.Models;
using Bank_Configuration_Portal.Resources;
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
        public ActionResult CreateOrResetUser()
        {
            var model = new MaintenanceViewModel
            {
                Secret = Request["secret"]
            };
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

                Startup.RevokeStamp(model.UserName, model.BankId.Value.ToString());

                ViewBag.GeneratedPassword = pwd;
                ViewBag.Created = created;
                ViewBag.UserName = model.UserName;
                ViewBag.BankId = model.BankId;


                return View(model);
            }
            catch (System.Exception ex)
            {
                Logger.LogError(ex, "MaintenanceController.CreateOrResetUser");
                ModelState.AddModelError("", Language.Generic_Error);
                return View(model);
            }
        }
    }
}
