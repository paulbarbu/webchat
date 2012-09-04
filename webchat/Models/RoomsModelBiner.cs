using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Diagnostics;

namespace webchat.Models {
    public class RoomsModelBiner : IModelBinder {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext) {
            Rooms rooms = new Rooms(
                controllerContext.HttpContext.Request["rooms"]
                .Trim()
                .Split(" ".ToCharArray())
            );

            //TODO: uniquify list, see what else I did in py!
            
            return rooms;
        }
    }
}