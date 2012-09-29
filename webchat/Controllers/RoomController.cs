using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webchat.Database;
using webchat.Filters;
using webchat.Helpers;
using webchat.Models;
using webchat.Validators;

namespace webchat.Controllers
{
    /// <summary>
    /// Handle the joning and leaving of rooms (channels)
    /// </summary>
    /// <remarks>In order to be able to join or leave rooms the user must be authenticated 
    /// <seealso cref="AuthenticationFilterAttribute"/></remarks>
    [AuthenticationFilter]
    public class RoomController : Controller
    {
        /// <summary>
        /// Join a list of rooms
        /// </summary>
        /// <param name="roomsModel">The List of rooms to be joined</param>
        /// <returns>Returns the user's currently joined rooms as JSON</returns>
        /// <remarks>This may also return a localized error string if the room names are invalid 
        /// <seealso cref="RoomsValidationAttribute"/></remarks>
        [HttpPost]
        [ValidateInput(false)]
        public string Join(RoomsModel roomsModel)
        {
            if(!ModelState.IsValid) {                
                Response.StatusCode = 400; // Bad Request
                
                return Resources.Strings.CharRoomsError;   
            }

            if(roomsModel.Rooms[0] == "") {
                return "";
            }

            MvcApplication.Db.AddUser(roomsModel.Rooms, (string)Session["nick"]);

            roomsModel.Rooms.Clear();
            roomsModel.Rooms.AddRange(MvcApplication.Db.GetRooms((string)Session["nick"]));

            return JsonConvert.SerializeObject(roomsModel.Rooms);
        }

        /// <summary>
        /// Leave a room
        /// </summary>
        /// <param name="leaveModel">The room to leave</param>
        /// <returns>If the user is still connected on other rooms returns them as JSON 
        /// or returns "" if the user left the only room he was connected to</returns>
        /// <remarks>This may also return a localized error string if the user tries 
        /// to leave a room he's not connected to 
        /// <seealso cref="JoinedRoomValidationAttribute"/></remarks>
        [HttpPost]
        [ValidateInput(false)]
        public string Leave(LeaveModel leaveModel) {
            if(!ModelState.IsValid) {
                Response.StatusCode = 400; // Bad Request                
                return Resources.Strings.CharRoomsError;
            }

            List<string> currentRooms = new List<string> { leaveModel.Room };

            MvcApplication.Db.DelUser(currentRooms, (string)Session["nick"]);
            

            currentRooms.Clear();
            currentRooms.AddRange(MvcApplication.Db.GetRooms((string)Session["nick"]));

            if(0 == currentRooms.Count){
                MvcApplication.Db.DelUserFromGlobalList((string)Session["nick"]);
                Session.Abandon();
                Response.StatusCode = 404;
                return "";
            }
                            
            return JsonConvert.SerializeObject(currentRooms);
        }
    }
}
