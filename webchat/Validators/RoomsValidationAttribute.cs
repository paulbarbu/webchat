using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using webchat.Models;

namespace webchat.Validators {
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class RoomsValidationAttribute : ValidationAttribute {
        public RoomsValidationAttribute()
            : base(Resources.Strings.CharRoomsError) {
        }

        public override bool IsValid(object value) {
            Rooms rooms = (Rooms)value;

            if(1 == rooms.Count && "" == rooms[0]){
                return true;
            }

            Match m;

            foreach(var room in rooms) {
                m = Regex.Match(room, @"^[\w ]+$", RegexOptions.Compiled);

                if(!m.Success) {
                    return false;
                }
            }

            return true;
        }
    }
}