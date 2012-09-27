using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webchat.Helpers;

namespace webchat.Models {
    public class MessageModelBinder : DefaultModelBinder {
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, System.ComponentModel.PropertyDescriptor propertyDescriptor) {

            if(propertyDescriptor.Name == "Message") {
                MessageModel model = (MessageModel)bindingContext.Model;
                    
                model.Message = BindingHelper.GetValue<string>(bindingContext, "message");

                if(null == model.Message) {
                    model.Message = "";
                }
                else {
                    model.Message = model.Message.Trim();
                    model.Message = HttpUtility.HtmlEncode(model.Message);
                }
            }
            else {
                base.BindProperty(controllerContext, bindingContext, propertyDescriptor);
            }
        }
    }
}