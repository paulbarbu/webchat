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

    [AuthenticationFilter]
    public class RoomController : Controller
    {
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

            Db.AddUser(roomsModel.Rooms, (string)Session["nick"]);

            Publisher.Publish(Resources.Strings.UsersEventChannel,
                JsonConvert.SerializeObject(Db.GetUsers()));

            roomsModel.Rooms.Clear();
            roomsModel.Rooms.AddRange(Db.GetRooms((string)Session["nick"]));

            return JsonConvert.SerializeObject(roomsModel.Rooms);
        }

        [HttpPost]
        [ValidateInput(false)]
        public string Leave(LeaveModel leaveModel) {
            if(!ModelState.IsValid) {
                Response.StatusCode = 400; // Bad Request                
                return Resources.Strings.CharRoomsError;
            }

            List<string> currentRooms = new List<string> { leaveModel.Room };
          
            Db.DelUser(currentRooms, (string)Session["nick"]);

            Publisher.Publish(Resources.Strings.UsersEventChannel,
                JsonConvert.SerializeObject(Db.GetUsers()));

            currentRooms.Clear();
            currentRooms.AddRange(Db.GetRooms((string)Session["nick"]));

            if(0 == currentRooms.Count){
                Session.Abandon();
                Response.StatusCode = 404;
                return "";
            }
                
            return JsonConvert.SerializeObject(currentRooms);
        }
    }
}
