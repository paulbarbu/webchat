using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webchat.Helpers;

namespace webchat.Models.Binders {
    public class RoomsModelBinder : DefaultModelBinder {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor) {

            if(propertyDescriptor.Name == "Rooms") {
                RoomsModel model = (RoomsModel)bindingContext.Model;

                model.Rooms = BindingHelper.GetValue<string>(bindingContext, "rooms")
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