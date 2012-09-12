using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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

            try {
                joinModel.Rooms.AddUser((string)Session["nick"]);
                joinModel.Rooms.Notify();

                joinModel.Rooms.Update((string)Session["nick"]);
            }
            catch(RedisException e) {
                Logger.Log(Resources.Strings.DatabaseError, "ERROR");
                Logger.Log(e.ToString(), "ERROR");

                Response.StatusCode = 500; // Internal Error
                return Resources.Strings.DatabaseError;
            }

            return JsonConvert.SerializeObject(joinModel.Rooms);
        }

        [HttpPost]
        [ValidateInput(false)]
        public string Leave(LeaveModel leaveModel) {
            if(!ModelState.IsValid) {
                Response.StatusCode = 400; // Bad Request                
                return Resources.Strings.CharRoomsError;
            }

            Rooms currentRooms = null;
            
            try {
                currentRooms = new Rooms(new[]{leaveModel.Room});
          
                currentRooms.DelUser((string)Session["nick"]);
                currentRooms.Notify();

                currentRooms.Update((string)Session["nick"]);

                if(0 == currentRooms.Count){
                    Session.Abandon();
                    Response.StatusCode = 404;
                    return "";
                }
            }
            catch(RedisException e) {
                Logger.Log(Resources.Strings.DatabaseError, "ERROR");
                Logger.Log(e.ToString(), "ERROR");

                Response.StatusCode = 500; // Internal Error
                return Resources.Strings.DatabaseError;
            }
    
            return JsonConvert.SerializeObject(currentRooms);
        }
    }
}
