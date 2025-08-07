using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal.Controllers
{
    public class ErrorController : BaseController
    {
        [AllowAnonymous]
        public ActionResult Startup()
        {
            string error = HttpContext.Application["StartupError"]?.ToString() ?? "Unknown startup error.";
            ViewBag.ErrorMessage = error;
            return View();
        }
    }

}