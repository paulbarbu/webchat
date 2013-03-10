using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webchat.Helpers;

namespace webchat.Models.Binders {
    /// <summary>
    /// Binder for <see cref="RoomsModel"/>
    /// </summary>
    public class RoomsModelBinder : DefaultModelBinder {
        /// <summary>
        /// Overridden method to customly bind a string to a List&lt;string&gt; after splitting and sanitizing it
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        /// <param name="propertyDescriptor"></param>
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor) {

            if(propertyDescriptor.Name == "Rooms") {
                RoomsModel model = (RoomsModel)bindingContext.Model;

                model.Rooms = BindingHelper.GetValue<string>(bindingContext, "Rooms.Rooms")
                    .Trim()
                    .Split(" ".ToCharArray())
                    .Distinct()
                    .ToList();
            }
            else {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }
    }
}