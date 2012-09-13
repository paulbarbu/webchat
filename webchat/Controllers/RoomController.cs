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

            joinModel.Rooms.AddUser((string)Session["nick"]);

            Publisher.Publish(Resources.Strings.UsersEventChannel,
                JsonConvert.SerializeObject(joinModel.Rooms.GetUsers()));

            joinModel.Rooms.Update((string)Session["nick"]);

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
          
            currentRooms.DelUser((string)Session["nick"]);

            Publisher.Publish(Resources.Strings.UsersEventChannel,
                JsonConvert.SerializeObject(currentRooms.GetUsers()));

            currentRooms.Update((string)Session["nick"]);

            if(0 == currentRooms.Count){
                Session.Abandon();
                Response.StatusCode = 404;
                return "";
            }
                
            return JsonConvert.SerializeObject(currentRooms);
        }
    }
}
