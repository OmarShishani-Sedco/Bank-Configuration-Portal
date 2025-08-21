using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Bank_Configuration_Portal
{
    public static class FilterHtmlExtensions
    {
        /// <summary>
        /// Strongly-typed helper: renders Label + DropDownListFor for a nullable bool (IsActive filter).
        /// </summary>
        public static IHtmlString ActiveFilterDropDownFor<TModel>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, bool?>> expression,
            string labelText,
            string selectText,
            string activeText,
            string inactiveText,
            object htmlAttributes = null)
        {
            var label = html.LabelFor(expression, labelText, new { @class = "form-label" }).ToHtmlString();

            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var current = metadata.Model as bool?;

            var items = new List<SelectListItem>
            {
                new SelectListItem { Value = "",     Text = selectText,  Selected = current == null  },
                new SelectListItem { Value = "true", Text = activeText,  Selected = current == true  },
                new SelectListItem { Value = "false",Text = inactiveText,Selected = current == false },
            };

            var attrs = EnsureSelectClasses(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
            var dropdown = html.DropDownListFor(expression, items, attrs).ToHtmlString();

            return MvcHtmlString.Create($"<div class=\"mb-3\">{label}{dropdown}</div>");
        }


        // ---- helpers ----
        private static IDictionary<string, object> EnsureSelectClasses(IDictionary<string, object> attrs)
        {
            if (attrs == null) attrs = new Dictionary<string, object>();
            // Ensure Bootstrap select + your glass background
            if (attrs.ContainsKey("class")) attrs["class"] = attrs["class"] + " form-select glass-bg";
            else attrs["class"] = "form-select glass-bg";
            return attrs;
        }
    }
}
