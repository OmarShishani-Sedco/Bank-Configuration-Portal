using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal
{
    public static class ButtonHtmlExtensions
    {
        /// <summary>Primary submit button with optional icon.</summary>
        public static IHtmlString PrimaryButton(this HtmlHelper html, string text, string iconClass = null, string extraClasses = null)
        {
            var icon = string.IsNullOrWhiteSpace(iconClass) ? "" : $"<i class=\"{HttpUtility.HtmlAttributeEncode(iconClass)}\"></i> ";
            return MvcHtmlString.Create(
                $"<button type=\"submit\" class=\"btn btn-primary {(extraClasses ?? "")}\">{icon}{HttpUtility.HtmlEncode(text)}</button>");
        }

        /// <summary>Secondry submit button with optional icon.</summary>
        public static IHtmlString SecondaryButton(this HtmlHelper html, string text, string iconClass = null, string extraClasses = null)
        {
            var icon = string.IsNullOrWhiteSpace(iconClass) ? "" : $"<i class=\"{HttpUtility.HtmlAttributeEncode(iconClass)}\"></i> ";
            return MvcHtmlString.Create(
                $"<button type=\"submit\" class=\"btn btn-secondary {(extraClasses ?? "")}\">{icon}{HttpUtility.HtmlEncode(text)}</button>");
        }

        public static IHtmlString SecondaryLinkButton(this HtmlHelper html, string text, string url, string iconClass = null, string extraClasses = null)
        {
            var icon = string.IsNullOrWhiteSpace(iconClass) ? "" : $"<i class=\"{System.Web.HttpUtility.HtmlAttributeEncode(iconClass)}\"></i> ";
            return new MvcHtmlString(
                $"<a href=\"{System.Web.HttpUtility.HtmlAttributeEncode(url)}\" class=\"btn btn-secondary {(extraClasses ?? "")}\">{icon}{System.Web.HttpUtility.HtmlEncode(text)}</a>");
        }


        public static IHtmlString DangerButton(this HtmlHelper html, string text, string iconClass = null, string extraClasses = null)
        {
            var icon = string.IsNullOrWhiteSpace(iconClass) ? "" : $"<i class=\"{HttpUtility.HtmlAttributeEncode(iconClass)}\"></i> ";
            return MvcHtmlString.Create(
                $"<button type=\"submit\" class=\"btn btn-danger {(extraClasses ?? "")}\">{icon}{HttpUtility.HtmlEncode(text)}</button>");
        }
    }
}
