using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal
{
    public static class ConflictHtmlExtensions
    {
        /// <summary>
        /// Renders a Bootstrap alert when `show` is true. Use for optimistic-concurrency conflict UI.
        /// </summary>
        public static IHtmlString ConflictNotice(this HtmlHelper html,
            bool show,
            string title,
            string prompt = null,
            bool center = true)
        {
            if (!show) return MvcHtmlString.Empty;

            var cls = $"alert alert-warning mb-4" +
                      (center ? " text-center" : "");

            var titleHtml = string.IsNullOrWhiteSpace(title) ? "" : $"<p class=\"mb-2\"><strong>{HttpUtility.HtmlEncode(title)}</strong></p>";
            var promptHtml = string.IsNullOrWhiteSpace(prompt) ? "" : $"<small>{HttpUtility.HtmlEncode(prompt)}</small>";

            return MvcHtmlString.Create($"<div class=\"{cls}\">{titleHtml}{promptHtml}</div>");
        }
    }
}
