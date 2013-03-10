using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using webchat.Validators;
using System.Web.Mvc;

namespace webchat.Models {
    /// <summary>
    /// Model for the <see cref="Controllers.IndexController"/>
    /// </summary>
    public class IndexModel {
        /// <summary>
        /// The user's nickname
        /// </summary>
        [Required(ErrorMessageResourceName = "RequiredNickError",
            ErrorMessageResourceType=typeof(Resources.Strings))]
        [StringLength(30, MinimumLength = 3, ErrorMessageResourceName = "LengthNickError",
            ErrorMessageResourceType = typeof(Resources.Strings))]
        [RegularExpression(@"^[\w]+$", ErrorMessageResourceName = "CharNickError",
            ErrorMessageResourceType = typeof(Resources.Strings))]
        [UniqueNickValidation]
        public string Nick { get; set; }

        /// <summary>
        /// The rooms that the user wants to connect to
        /// </summary>
        public RoomsModel Rooms {get; set;}

        /// <summary>
        /// Rooms that are already populated by users
        /// </summary>
        public RoomsModel AvailableRooms { get; set; }
    }
}