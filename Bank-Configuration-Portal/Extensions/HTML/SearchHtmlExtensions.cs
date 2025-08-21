using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Bank_Configuration_Portal
{
    public static class SearchHtmlExtensions
    {
        /// <summary>
        /// Label + TextBoxFor styled as a search box (no input-group, matches your original view).
        /// </summary>
        public static IHtmlString SearchBoxFor<TModel>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, string>> expression,
            string labelText,
            string placeholder = null,
            object htmlAttributes = null)
        {
            var label = html.LabelFor(expression, labelText, new { @class = "form-label" }).ToHtmlString();

            var attrs = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes ?? new { });
            if (attrs.ContainsKey("class")) attrs["class"] = attrs["class"] + " form-control glass-bg";
            else attrs["class"] = "form-control glass-bg";

            attrs["type"] = "search";
            attrs["autocomplete"] = "off";
            if (!string.IsNullOrEmpty(placeholder)) attrs["placeholder"] = placeholder;

            var input = html.TextBoxFor(expression, attrs).ToHtmlString();

            return MvcHtmlString.Create($"<div class=\"mb-3\">{label}{input}</div>");
        }
    }
}
