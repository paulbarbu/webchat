using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webchat.Models.Binders;
using webchat.Validators;

namespace webchat.Models {
    /// <summary>
    /// Model used by MessageController
    /// </summary>
    [ModelBinder(typeof(MessageModelBinder))]
    public class MessageModel {
        /// <summary>
        /// Users message
        /// </summary>
        [Required]
        [StringLength(Int32.MaxValue, MinimumLength=1)]
        public string Message { get; set; }

        /// <summary>
        /// The room on which the message was sent
        /// </summary>
        [JoinedRoomValidation]
        public string Room { get; set; }
    }
}