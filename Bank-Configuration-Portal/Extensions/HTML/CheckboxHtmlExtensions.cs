using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Bank_Configuration_Portal
{
    public static class CheckboxHtmlExtensions
    {
        /// <summary>
        /// Renders a checkbox + label. In readOnly mode, shows a disabled checkbox and posts a hidden mirror value.
        /// </summary>
        public static MvcHtmlString CheckBoxOrReadonlyFor<TModel>(this HtmlHelper<TModel> html,
            Expression<Func<TModel, bool>> expression,
            bool readOnly,
            object checkboxHtmlAttributes = null,
            object labelHtmlAttributes = null)
        {
            if (!readOnly)
            {
                var cb = html.CheckBoxFor(expression, MergeClass(checkboxHtmlAttributes, "form-check-input")).ToHtmlString();
                var lbl = html.LabelFor(expression, labelHtmlAttributes ?? new { @class = "form-check-label" }).ToHtmlString();
                return MvcHtmlString.Create(cb + lbl);
            }

            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            bool isChecked = (metadata.Model as bool?) ?? false;

            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var fieldId = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(fieldName);

            var tag = new TagBuilder("input");
            tag.Attributes["type"] = "checkbox";
            tag.Attributes["id"] = fieldId;
            tag.Attributes["disabled"] = "disabled";
            if (isChecked) tag.Attributes["checked"] = "checked";
            MergeHtmlAttributes(tag, MergeClass(checkboxHtmlAttributes, "form-check-input"));

            var lblReadOnly = html.LabelFor(expression, labelHtmlAttributes ?? new { @class = "form-check-label" }).ToHtmlString();
            var hiddenMirror = html.HiddenFor(expression).ToHtmlString();

            return MvcHtmlString.Create(tag.ToString(TagRenderMode.SelfClosing) + lblReadOnly + hiddenMirror);
        }

        private static IDictionary<string, object> MergeClass(object attrs, string append)
        {
            var dict = HtmlHelper.AnonymousObjectToHtmlAttributes(attrs ?? new { });
            if (dict.ContainsKey("class")) dict["class"] = dict["class"] + " " + append;
            else dict["class"] = append;
            return dict;
        }

        private static void MergeHtmlAttributes(TagBuilder tag, IDictionary<string, object> attrs)
        {
            foreach (var kv in attrs) tag.MergeAttribute(kv.Key, kv.Value?.ToString(), replaceExisting: kv.Key == "class");
        }
    }
}
