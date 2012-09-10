using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace webchat.Helpers {
    public class BindingHelpers {
        /**
         * http://odetocode.com/blogs/scott/archive/2009/05/05/iterating-on-an-asp-net-mvc-model-binder.aspx
         */
        public static T GetValue<T>(ModelBindingContext bindingContext, string key) {
            ValueProviderResult valueResult;

            valueResult = bindingContext.ValueProvider.GetValue(key);            
            bindingContext.ModelState.SetModelValue(key, valueResult);

            return (T)valueResult.ConvertTo(typeof(T));
        } 
    }
}