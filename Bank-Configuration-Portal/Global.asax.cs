using Bank_Configuration_Portal.App_Start;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            if (!DatabaseUtility.TestConnection(out string errorMessage))
            {
                Application["StartupError"] = errorMessage;
                return;
            }
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            DIConfig.RegisterServices();
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
