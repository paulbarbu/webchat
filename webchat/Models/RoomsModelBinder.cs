using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace webchat.Models {
    public class RoomsModelBinder : IModelBinder {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            Rooms rooms = new Rooms(
                GetValue<string>(bindingContext, "rooms")
                .Trim()
                .Split(" ".ToCharArray())
            );
            
            if(0 == rooms.Count) {
                rooms.Add("default");
            }
            /*TODO: if I implement an internface this is the right this to do afterwards:
             *  IBindingValidatable validatableObject = bindingContext.Model as IBindingValidatable;
            if (validatableObject != null)
                validatableObject.Validate(bindingContext.ModelState);
             */
            //bindingContext.ModelState.AddModelError("rooms", "You're screwed");

            //rooms.Add("bleach");

            return rooms;//.Distinct(); //TODO: uncomment after implementing the validation
        }

        /**
         * http://odetocode.com/blogs/scott/archive/2009/05/05/iterating-on-an-asp-net-mvc-model-binder.aspx
         */
        private T GetValue<T>(ModelBindingContext bindingContext, string key) {
            ValueProviderResult valueResult;
            valueResult = bindingContext.ValueProvider.GetValue(key);
            bindingContext.ModelState.SetModelValue(key, valueResult);
            return (T)valueResult.ConvertTo(typeof(T));
        }  
    }
}