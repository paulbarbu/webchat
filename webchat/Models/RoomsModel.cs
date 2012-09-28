using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webchat.Models.Binders;
using webchat.Validators;

namespace webchat.Models {
    [ModelBinder(typeof(RoomsModelBinder))]
    public class RoomsModel {
        [RoomsValidation]
        public List<string> Rooms { get; set; }
    }
}