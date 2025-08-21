using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Bank_Configuration_Portal
{
    public static class FormHtmlExtensions
    {
        /// <summary>BeginForm + AntiForgery in one call.</summary>
        public static MvcForm BeginFormAntiForgery(this HtmlHelper html,
            string action, string controller, FormMethod method = FormMethod.Post, object htmlAttributes = null)
        {
            var form = html.BeginForm(action, controller, method, htmlAttributes);
            html.ViewContext.Writer.Write(html.AntiForgeryToken().ToHtmlString());
            return form;
        }
    }
}
