using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using webchat.Validators;
using System.Web.Mvc;

namespace webchat.Models {
    /// <summary>
    /// Model for the IndexController
    /// </summary>
    public class IndexModel : RoomsModel {
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
    }
}