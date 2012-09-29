using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace webchat.Helpers {
    /// <summary>
    /// Helper to display errors
    /// </summary>
    public static class ErrorSummaryHelper {
        /// <summary>
        /// Display errors using some default settings
        /// </summary>
        /// <param name="helper">The helper object</param>
        /// <param name="id">The element's ID attribute</param>
        /// <param name="errors">A list of string errors to be displayed</param>
        /// <returns>Returns an <see cref="IHtmlString"/> containing the element's code</returns>
        public static IHtmlString ErrorSummary(this HtmlHelper helper, string id, List<string> errors) {
            return ErrorSummary(helper, id, errors, null, null);
        }

        /// <summary>
        /// Display errors using custom settings
        /// </summary>
        /// <param name="helper">The helper object</param>
        /// <param name="id">The element's ID attribute</param>
        /// <param name="errors">A list of string errors to be displayed</param>
        /// <param name="outerHtmlAttributes">An annonymous object containing the outer's element attributes</param>
        /// <param name="innertHtmlAttributes">An annonymous object containing the inner's element attributes</param>
        /// <returns>Returns an <see cref="IHtmlString"/> containing the element's code</returns>
        public static IHtmlString ErrorSummary(this HtmlHelper helper, string id, List<string> errors,
            object outerHtmlAttributes, object innertHtmlAttributes) {
            
            TagBuilder containerBuilder = new TagBuilder("div");

            containerBuilder.GenerateId(id);
            containerBuilder.MergeAttributes(new RouteValueDictionary(outerHtmlAttributes));

            foreach(var error in errors){
                TagBuilder errorBuilder = new TagBuilder("div");

                errorBuilder.MergeAttributes(new RouteValueDictionary(innertHtmlAttributes));
                errorBuilder.SetInnerText(error);

                containerBuilder.InnerHtml += errorBuilder.ToString();
            }

            return new HtmlString(containerBuilder.ToString());
        }
    }
}