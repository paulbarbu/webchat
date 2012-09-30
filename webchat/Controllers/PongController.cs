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
    /// <summary>
    /// Handles the PONG part of the PING PONG protocol
    /// </summary>
    /// <remarks>In order to be able to respond to pings the user must be authenticated 
    /// <seealso cref="Filters.AuthenticationFilterAttribute"/></remarks>
    [AuthenticationFilter]
    public class PongController : Controller
    {
        /// <summary>
        /// Restore the user's state in the application if he has an active connection to it
        /// </summary>
        /// <returns>Returns a HttpStatusCode which represents 
        /// whether the opperation was successful or not</returns>
        [HttpPost]
        public HttpStatusCode Index()
        {
            if(!ModelState.IsValid) {
                return HttpStatusCode.BadRequest;
            }

            string nick = (string)Session["nick"];
            List<string> rooms = MvcApplication.Db.GetBackupRooms(nick);
            MvcApplication.Db.AddUser(rooms, nick);

            return HttpStatusCode.OK;
        }

    }
}
