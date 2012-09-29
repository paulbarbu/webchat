using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using webchat.Models;

namespace webchat.Validators {
    /// <summary>
    /// Check the room names
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=false)]
    public class RoomsValidationAttribute : ValidationAttribute {
        /// <summary>
        /// The constructor
        /// </summary>
        public RoomsValidationAttribute()
            : base(Resources.Strings.CharRoomsError) {
        }

        /// <summary>
        /// Do the actual checking against the rooms
        /// </summary>
        /// <param name="value">A List&lt;string&gt; of rooms to be checked</param>
        /// <returns>Returns true if all rooms are valid, else false</returns>
        /// <remarks>A room name is considered valid if it matches the regex: ^[\w]+$</remarks>
        public override bool IsValid(object value) {
            List<string> rooms = (List<string>)value;
            
            if(1 == rooms.Count && "" == rooms[0].Trim()){
                return true;
            }

            Match m;

            foreach(var room in rooms) {
                m = Regex.Match(room, @"^[\w]+$", RegexOptions.Compiled);

                if(!m.Success) {
                    return false;
                }
            }

            return true;
        }
    }
}