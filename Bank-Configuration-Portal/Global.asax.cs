using AutoMapper;
using Bank_Configuration_Portal.App_Start;
using Bank_Configuration_Portal.Common;
using Bank_Configuration_Portal.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Bank_Configuration_Portal
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            AntiForgeryConfig.UniqueClaimTypeIdentifier = ClaimsIdentity.DefaultNameClaimType;
            AutofacConfig.ConfigureContainer();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            WindowsEventLogger.TryEnsureSource();
            if (!DatabaseUtility.TestConnection(out string errorMessage))
            {
                Application["StartupError"] = errorMessage;
                return;
            }
            BundleConfig.RegisterBundles(BundleTable.Bundles);

        }
        protected void Application_BeginRequest()
        {
            if (Application["StartupError"] != null)
            {
                // Only redirect once
                if (!HttpContext.Current.Request.Url.AbsolutePath.Contains("/Error/Startup"))
                {
                    HttpContext.Current.Response.Redirect("~/Error/Startup");
                }
            }
        }

    }
}
