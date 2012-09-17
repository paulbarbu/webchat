using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using webchat.Database;
using webchat.Helpers;
using webchat.Models;

namespace webchat.Validators {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class JoinedRoomValidationAttribute : ValidationAttribute {
        public JoinedRoomValidationAttribute()
            : base(Resources.Strings.CharRoomsError) {
        }

        public override bool IsValid(object value) {
            List<string> rooms;

            lock(Locker.locker) {
                rooms = Db.GetRooms((string)HttpContext.Current.Session["nick"]);
            }

            string room = (string)value;

            if(-1 != rooms.IndexOf(room)) {
                return true;
            }

            return false;
        }
    }
}