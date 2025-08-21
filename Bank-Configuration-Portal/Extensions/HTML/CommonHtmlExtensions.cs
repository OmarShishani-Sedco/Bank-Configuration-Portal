using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Bank_Configuration_Portal
{
    public static class CommonHtmlExtensions
    {
        /// <summary>Compact validation summary with default styling.</summary>
        public static IHtmlString ValidationSummaryBlock(this HtmlHelper html,
            bool excludePropertyErrors = true,
            string cssClass = "text-danger fadeout3s")
        {
            return html.ValidationSummary(excludePropertyErrors, "", new { @class = cssClass });
        }

        /// <summary>Renders TempData -> Bootstrap alerts for Success/Error/Info.</summary>
        public static IHtmlString AlertsFromTempData(this HtmlHelper html)
        {
            var temp = html.ViewContext.TempData;
            var parts = new List<string>();

            void add(string key, string css)
            {
                if (temp[key] != null)
                {
                    var msg = HttpUtility.HtmlEncode(temp[key].ToString());
                    parts.Add($"<div class=\"alert {css} fadeout3s\">{msg}</div>");
                }
            }

            add("Success", "alert-success");
            add("Error", "alert-danger");
            add("Info", "alert-info");

            return MvcHtmlString.Create(string.Join("", parts));
        }
    }
}
