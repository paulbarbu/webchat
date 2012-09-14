using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using webchat.Database;
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

            string nick = (string)Session["nick"];

            List<string> rooms = Db.GetBackupRooms(nick);

            lock(Locker.locker) {
                Db.AddUser(rooms, nick);

                Publisher.Publish(Resources.Strings.UsersEventChannel, JsonConvert.SerializeObject(Db.GetUsers()));
            }

            return HttpStatusCode.OK;
        }

    }
}
