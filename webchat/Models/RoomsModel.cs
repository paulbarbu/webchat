using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webchat.Models.Binders;
using webchat.Validators;

namespace webchat.Models {
    /// <summary>
    /// Model used by IndexController and RoomController for joining rooms
    /// </summary>
    [ModelBinder(typeof(RoomsModelBinder))]
    public class RoomsModel {
        /// <summary>
        /// The rooms joined
        /// </summary>
        [RoomsValidation]
        public List<string> Rooms { get; set; }
    }
}