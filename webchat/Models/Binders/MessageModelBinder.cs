using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webchat.Helpers;

namespace webchat.Models.Binders {
    /// <summary>
    /// Binder for <see cref="MessageModel"/>
    /// </summary>
    public class MessageModelBinder : DefaultModelBinder {
        /// <summary>
        /// Overridden method to customly bind a string by trimming and escaping it
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        /// <param name="propertyDescriptor"></param>
        protected override void BindProperty(ControllerContext controllerContext, ModelBindingContext bindingContext, PropertyDescriptor propertyDescriptor) {

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