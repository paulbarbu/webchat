using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using webchat.Validators;

namespace webchat.Models {
    public class ChatModel {
        [Required]
        public string Text { get; set; }
        [RoomsValidation]
        public Rooms Rooms { get; set; }
    }
}