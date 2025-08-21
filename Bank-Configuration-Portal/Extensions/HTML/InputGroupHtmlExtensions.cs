using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Bank_Configuration_Portal
{
    public static class InputGroupHtmlExtensions
    {
        /// <summary>Label + icon + TextBoxFor + ValidationMessage (Bootstrap input-group).</summary>
        public static IHtmlString InputGroupTextFor<TModel, TProp>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProp>> expression,
            string iconClass,
            string placeholder = null,
            int? maxlength = null,
            object htmlAttributes = null)
        {
            var label = html.LabelFor(expression, new { @class = "form-label" }).ToHtmlString();

            var attrs = MergeControlAttributes(htmlAttributes, placeholder, maxlength);
            var input = html.TextBoxFor(expression, attrs).ToHtmlString();

            var group = $"<div class=\"input-group\">" +
                        $"  <span class=\"input-group-text\"><i class=\"{HttpUtility.HtmlAttributeEncode(iconClass)}\"></i></span>" +
                        $"  {input}" +
                        $"</div>";

            var validation = html.ValidationMessageFor(expression, "", new { @class = "text-danger" }).ToHtmlString();

            return MvcHtmlString.Create($"<div class=\"mb-3\">{label}{group}{validation}</div>");
        }

        /// <summary>Label + icon + PasswordFor + ValidationMessage (Bootstrap input-group).</summary>
        public static IHtmlString InputGroupPasswordFor<TModel>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, string>> expression,
            string iconClass,
            string placeholder = null,
            int? maxlength = null,
            object htmlAttributes = null)
        {
            var label = html.LabelFor(expression, new { @class = "form-label" }).ToHtmlString();

            var attrs = MergeControlAttributes(htmlAttributes, placeholder, maxlength);
            var input = html.PasswordFor(expression, attrs).ToHtmlString();

            var group = $"<div class=\"input-group\">" +
                        $"  <span class=\"input-group-text\"><i class=\"{HttpUtility.HtmlAttributeEncode(iconClass)}\"></i></span>" +
                        $"  {input}" +
                        $"</div>";

            var validation = html.ValidationMessageFor(expression, "", new { @class = "text-danger" }).ToHtmlString();

            return MvcHtmlString.Create($"<div class=\"mb-3\">{label}{group}{validation}</div>");
        }

        /// <summary>
        /// Label + icon + number input (type=number) + ValidationMessage (Bootstrap input-group).
        /// Supports min/max/step and any extra htmlAttributes.
        /// </summary>
        public static IHtmlString InputGroupNumberFor<TModel, TProp>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProp>> expression,
            string iconClass,
            string placeholder = null,
            int? min = null,
            int? max = null,
            string step = null,
            object htmlAttributes = null)
        {
            var label = html.LabelFor(expression, new { @class = "form-label" }).ToHtmlString();

            var attrs = MergeNumberAttributes(htmlAttributes, placeholder, min, max, step);
            var input = html.TextBoxFor(expression, attrs).ToHtmlString();

            var group = $"<div class=\"input-group\">" +
                        $"  <span class=\"input-group-text\"><i class=\"{HttpUtility.HtmlAttributeEncode(iconClass)}\"></i></span>" +
                        $"  {input}" +
                        $"</div>";

            var validation = html.ValidationMessageFor(expression, "", new { @class = "text-danger" }).ToHtmlString();

            return MvcHtmlString.Create($"<div class=\"mb-3\">{label}{group}{validation}</div>");
        }

        /// <summary>
        /// Renders Label + icon + readonly TextBoxFor (+ optional HiddenFor mirror) + ValidationMessage.
        /// Use when you want to show the value but prevent editing (e.g., conflict mode).
        /// </summary>
        public static IHtmlString InputGroupTextReadOnlyFor<TModel, TProp>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, TProp>> expression,
            string iconClass,
            bool includeHiddenMirror = true)
        {
            var label = html.LabelFor(expression, new { @class = "form-label" }).ToHtmlString();

            var attrs = new Dictionary<string, object>
            {
                ["class"] = "form-control",
                ["readonly"] = "readonly"
            };

            var input = html.TextBoxFor(expression, attrs).ToHtmlString();
            var mirror = includeHiddenMirror ? html.HiddenFor(expression).ToHtmlString() : string.Empty;

            var group =
                $"<div class=\"input-group\">" +
                $"  <span class=\"input-group-text\"><i class=\"{HttpUtility.HtmlAttributeEncode(iconClass)}\"></i></span>" +
                $"  {input}" +
                $"</div>";

            var validation = html.ValidationMessageFor(expression, "", new { @class = "text-danger" }).ToHtmlString();

            return MvcHtmlString.Create($"<div class=\"mb-3\">{label}{group}{mirror}{validation}</div>");
        }

        // ---- helpers ----
        private static IDictionary<string, object> MergeControlAttributes(object htmlAttributes, string placeholder, int? maxlength)
        {
            var dict = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes ?? new { });

            // ensure Bootstrap class
            if (dict.ContainsKey("class")) 
                dict["class"] = dict["class"] + " form-control";
            else 
                dict["class"] = "form-control";

            if (!string.IsNullOrEmpty(placeholder))
                dict["placeholder"] = placeholder;

            if (maxlength.HasValue)
                dict["maxlength"] = maxlength.Value;

            return dict;
        }

        private static IDictionary<string, object> MergeNumberAttributes(object htmlAttributes, string placeholder, int? min, int? max, string step)
        {
            var dict = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes ?? new { });

            // ensure Bootstrap class
            if (dict.ContainsKey("class")) dict["class"] = dict["class"] + " form-control";
            else dict["class"] = "form-control";

            // force type=number
            dict["type"] = "number";

            if (!string.IsNullOrEmpty(placeholder))
                dict["placeholder"] = placeholder;

            if (min.HasValue) dict["min"] = min.Value;
            if (max.HasValue) dict["max"] = max.Value;
            if (!string.IsNullOrEmpty(step)) dict["step"] = step;

            dict["inputmode"] = "numeric";

            return dict;
        }
    }
}
