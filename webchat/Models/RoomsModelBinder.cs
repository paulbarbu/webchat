using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;
using webchat.Helpers;

namespace webchat.Models {
    public class RoomsModelBinder : IModelBinder {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            Rooms rooms = new Rooms(
                BindingHelpers.GetValue<string>(bindingContext, "rooms")
                .Trim()
                .Split(" ".ToCharArray())
                .Distinct()
                .ToArray()
            );

            return rooms;
        } 
    }
}