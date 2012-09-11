using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using webchat.Validators;

namespace webchat.Models {
    public class LeaveModel {
        [Required]
        [StringLength(Int32.MaxValue, MinimumLength=1)]
        [RegularExpression(@"^[\w]+$")]
        [JoinedRoomValidation]
        public string Room { get; set; }
    }
}