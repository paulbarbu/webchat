using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace webchat.Helpers {
    /// <summary>
    /// Binding helpers used by Models.Binders
    /// </summary>
    public class BindingHelper {
        /// <summary>
        /// Get a value from a HTTP form
        /// </summary>
        /// <typeparam name="T">Return's value type</typeparam>
        /// <param name="bindingContext">The object that holds all the HTTP data</param>
        /// <param name="key">The needed value should be retrieved by it's key (the element's name attribute)</param>
        /// <returns>Returns the value stored at the key</returns>
        /// <remarks>See: 
        /// http://odetocode.com/blogs/scott/archive/2009/05/05/iterating-on-an-asp-net-mvc-model-binder.aspx
        /// </remarks>
        public static T GetValue<T>(ModelBindingContext bindingContext, string key) {
            ValueProviderResult valueResult;

            valueResult = bindingContext.ValueProvider.GetValue(key);            
            bindingContext.ModelState.SetModelValue(key, valueResult);

            return (T)valueResult.ConvertTo(typeof(T));
        } 
    }
}