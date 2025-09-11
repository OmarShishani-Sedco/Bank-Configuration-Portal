using System.Web;
using System.Web.Optimization;

namespace Bank_Configuration_Portal
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate.js",
                        "~/Scripts/jquery.validate.unobtrusive.js",
                        "~/Scripts/validation.compare.js"));

            bundles.Add(new Bundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js")); 

            bundles.Add(new ScriptBundle("~/bundles/Custom").Include(
                        "~/Scripts/FadeOut.js",
                        "~/Scripts/modal_scripts.js",
                        "~/Scripts/Concurrency.js",
                        "~/Scripts/Disable_bfcache.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));
        }
    }
}
