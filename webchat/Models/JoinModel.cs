using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webchat.Validators;

namespace webchat.Models {
    [ModelBinder(typeof(JoinModelBinder))]
    public class JoinModel {
        [RoomsValidation(AllowEmpty=false)]
        public List<string> Rooms { get; set; }
    }
}