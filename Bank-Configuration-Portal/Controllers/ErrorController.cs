using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

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
        [AllowAnonymous]
        public ActionResult Generic(string errorMessage)
        {
            ViewBag.Title = "Application Error";
            ViewBag.ErrorMessage = errorMessage ?? "An unexpected error occurred. Please contact your system administrator.";
            return View();
        }

        [AllowAnonymous]
        public ActionResult NotFound()
        {
            ViewBag.Title = "Page Not Found";
            return View();
        }

        [AllowAnonymous]
        public ActionResult Forbidden()
        {
            ViewBag.Title = "Access Denied";
            return View();
        }
    }

}