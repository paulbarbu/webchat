using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace webchat.Models {
    public class IndexModel {
        [Required(ErrorMessage = "Please provide a valid nickname in order to connect!")]
        [StringLength(30, MinimumLength = 3,
            ErrorMessage = "Your nickname must be at least 3 characters long!")]
        [RegularExpression(@"^[\w]+$",
            ErrorMessage = "Your nickname must be composed only of alphanumeric characters and underscores!")]
        public string Nick { get; set; }

        [RegularExpression(@"^[\w ]+$",
            ErrorMessage = "Room names may be composed only of alphanumeric characters and underscores!")]
        public Rooms Rooms { get; set; }
    }
}