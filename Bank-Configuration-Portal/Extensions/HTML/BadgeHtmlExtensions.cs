using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal
{
    public static class BadgeHtmlExtensions
    {
        /// <summary>Boolean -> Bootstrap badge (active/inactive) with custom texts.</summary>
        public static IHtmlString ActiveBadge(this HtmlHelper html, bool isActive, string activeText, string inactiveText)
        {
            var text = isActive ? HttpUtility.HtmlEncode(activeText) : HttpUtility.HtmlEncode(inactiveText);
            var cls = isActive ? "badge bg-success" : "badge bg-secondary";
            return MvcHtmlString.Create($"<span class=\"{cls}\">{text}</span>");
        }

        /// <summary>Bootstrap badge (Nullable overload).</summary>
        public static IHtmlString ActiveBadge(this HtmlHelper html, bool? isActive, string activeText, string inactiveText, string nullText = "-")
        {
            if (!isActive.HasValue)
                return MvcHtmlString.Create($"<span class=\"badge bg-light text-muted\">{HttpUtility.HtmlEncode(nullText)}</span>");

            return ActiveBadge(html, isActive.Value, activeText, inactiveText);
        }
    }
}
