using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Bank_Configuration_Portal
{
    public static class ImageHtmlExtensions
    {
        public static IHtmlString Image(this HtmlHelper helper, string src, string alt, string cssClass = "", object htmlAttributes = null)
        {
            var tagBuilder = new TagBuilder("img");
            tagBuilder.MergeAttribute("src", UrlHelper.GenerateContentUrl(src, helper.ViewContext.HttpContext));
            tagBuilder.MergeAttribute("alt", alt);

            if (!string.IsNullOrEmpty(cssClass))
            {
                tagBuilder.AddCssClass(cssClass);
            }

            if (htmlAttributes != null)
            {
                var attributes = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                tagBuilder.MergeAttributes(attributes);
            }

            return new MvcHtmlString(tagBuilder.ToString(TagRenderMode.SelfClosing));
        }
    }
}