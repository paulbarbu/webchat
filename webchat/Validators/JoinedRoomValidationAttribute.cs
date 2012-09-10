using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using webchat.Models;

namespace webchat.Validators {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class JoinedRoomValidationAttribute : ValidationAttribute {
        public JoinedRoomValidationAttribute()
            : base(Resources.Strings.CharRoomsError) {
        }

        public override bool IsValid(object value) {
            Rooms rooms = null;

            try {
                rooms = new Rooms((string)HttpContext.Current.Session["nick"]);
            }
            catch(RedisException) {
                //TODO: log
                return false;
            }

            string room = (string)value;

            if(-1 != rooms.IndexOf(room)) {
                return true;
            }

            return false;
        }
    }
}