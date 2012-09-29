using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using webchat.Database;
using webchat.Helpers;
using webchat.Models;

namespace webchat.Validators {
    /// <summary>
    /// Checks whether the user has joined a certain room
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class JoinedRoomValidationAttribute : ValidationAttribute {
        /// <summary>
        /// The constructor
        /// </summary>
        public JoinedRoomValidationAttribute()
            : base(Resources.Strings.CharRoomsError) {
        }

        /// <summary>
        /// Do the actual checking of the room
        /// </summary>
        /// <param name="value">The room's name as a string</param>
        /// <returns>Returns true if the user joined the room, otherwise false</returns>
        public override bool IsValid(object value) {
            List<string> rooms;

            rooms = MvcApplication.Db.GetRooms((string)HttpContext.Current.Session["nick"]);
            
            string room = (string)value;

            if(-1 != rooms.IndexOf(room)) {
                return true;
            }

            return false;
        }
    }
}