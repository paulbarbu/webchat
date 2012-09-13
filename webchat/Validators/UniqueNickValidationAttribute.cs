using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using webchat.Database;

namespace webchat.Validators {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class UniqueNickValidationAttribute : ValidationAttribute {
        public UniqueNickValidationAttribute()
            : base(Resources.Strings.UniqueNickError) {
        }

        public override bool IsValid(object value) {
            if(null == value) {
                return true;
            }

            string nick = (string)value;

            if(Db.Users.Contains(nick)) {
                return false;
            }

            return true;
        }
    }
}