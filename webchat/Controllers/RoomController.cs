using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webchat.Filters;
using webchat.Models;
using webchat.Validators;

namespace webchat.Controllers
{

    [AuthenticationFilter]
    public class RoomController : Controller
    {
        [HttpPost]
        [ValidateInput(false)]
        public string Join(JoinLeaveModel joinModel)
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

                joinModel.Rooms.Clear();
                joinModel.Rooms.Update((string)Session["nick"]);
            }
            catch(RedisException) {
                //TODO: log
                Response.StatusCode = 500; // Internal Error
                return Resources.Strings.DatabaseError;
            }

            return JsonConvert.SerializeObject(joinModel.Rooms);
        }

    }
}
