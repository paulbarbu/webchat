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
        public string Join(JoinModel joinModel)
        {
            if(!ModelState.IsValid) {                
                Response.StatusCode = 400; // Bad Request
                
                if(joinModel.Rooms[0] == "") {
                    return "";
                }

                return Resources.Strings.CharRoomsError;   
            }

            Db.AddUser(joinModel.Rooms, (string)Session["nick"]);

            Publisher.Publish(Resources.Strings.UsersEventChannel,
                JsonConvert.SerializeObject(Db.GetUsers()));

            joinModel.Rooms.Clear();
            joinModel.Rooms.AddRange(Db.GetRooms((string)Session["nick"]));

            return JsonConvert.SerializeObject(joinModel.Rooms);
        }

        [HttpPost]
        [ValidateInput(false)]
        public string Leave(LeaveModel leaveModel) {
            if(!ModelState.IsValid) {
                Response.StatusCode = 400; // Bad Request                
                return Resources.Strings.CharRoomsError;
            }

            Rooms currentRooms = new Rooms(new[] { leaveModel.Room });
          
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
