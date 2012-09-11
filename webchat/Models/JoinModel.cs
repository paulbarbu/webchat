using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using webchat.Validators;

namespace webchat.Models {
    public class JoinModel {
        [RoomsValidation(AllowEmpty=false)]
        public Rooms Rooms { get; set; }
    }
}