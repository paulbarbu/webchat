using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webchat.Models.Binders;
using webchat.Validators;

namespace webchat.Models {
    [ModelBinder(typeof(MessageModelBinder))]
    public class MessageModel {
        [Required]
        [StringLength(Int32.MaxValue, MinimumLength=1)]
        public string Message { get; set; }

        [JoinedRoomValidation]
        public string Room { get; set; }
    }
}