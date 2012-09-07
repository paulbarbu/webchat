using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace webchat.Helpers {
    public static class ErrorSummaryHelper {
        public static IHtmlString ErrorSummary(this HtmlHelper helper, string id, List<string> errors) {
            return ErrorSummary(helper, id, errors, null, null);
        }

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