using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using webchat.Validators;

namespace webchat.Models {
    /// <summary>
    /// Model used by <see cref="Controllers.RoomController"/> when the user leaves rooms
    /// </summary>
    public class LeaveModel {
        /// <summary>
        /// The room the user left
        /// </summary>
        [Required]
        [StringLength(Int32.MaxValue, MinimumLength=1)]
        [RegularExpression(@"^[\w]+$")]
        [JoinedRoomValidation]
        public string Room { get; set; }
    }
}