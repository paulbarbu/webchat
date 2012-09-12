using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using webchat.Filters;
using webchat.Helpers;
using webchat.Models;

namespace webchat.Controllers
{
    [AuthenticationFilter]
    public class PongController : Controller
    {
        [HttpPost]
        public HttpStatusCode Index()
        {
            if(!ModelState.IsValid) {
                return HttpStatusCode.BadRequest;
            }

            try {
                using(var r = new RedisClient().As<string>()) {
                    var user_rooms = r.Sets[(string)Session["nick"]];

                    Rooms rooms = new Rooms(user_rooms.ToArray());
                    rooms.AddUser((string)Session["nick"]);
                    rooms.Notify();

                    r.RemoveEntry((string)Session["nick"]);
                }
            }
            catch(RedisException e) {
                Logger.Log(Resources.Strings.DatabaseError, "ERROR");
                Logger.Log(e.ToString(), "ERROR");

                return HttpStatusCode.InternalServerError;
            }

            return HttpStatusCode.OK;
        }

    }
}
