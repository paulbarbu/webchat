using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using webchat.Database;

namespace webchat.Validators {
    /// <summary>
    /// Check whether a nick is unique
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class UniqueNickValidationAttribute : ValidationAttribute {
        /// <summary>
        /// The constructor
        /// </summary>
        public UniqueNickValidationAttribute()
            : base(Resources.Strings.UniqueNickError) {
        }

        /// <summary>
        /// Do the actual checking of the uniqueness of the nick
        /// </summary>
        /// <param name="value">The user's nick as string</param>
        /// <returns>Return true if the nick is unique</returns>
        public override bool IsValid(object value) {
            if(null == value) {
                return true;
            }

            string nick = (string)value;

            if(MvcApplication.Db.IsUser(nick)) {
                return false;
            }

            return true;
        }
    }
}