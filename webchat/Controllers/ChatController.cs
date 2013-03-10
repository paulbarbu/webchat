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

namespace webchat.Controllers
{
    /// <summary>
    /// Handles the main page of the appliaction
    /// </summary>
    /// <remarks>In order to enter the chat the user must be authenticated 
    /// <seealso cref="Filters.AuthenticationFilterAttribute"/></remarks>
    [AuthenticationFilter]
    public class ChatController : Controller
    {
        /// <summary>
        /// Load the initial page where the user can send messages and join additional rooms
        /// </summary>
        /// <returns>Returns a <see cref="Models.ChatModel"/></returns>
        public ActionResult Index() {
            ChatModel model = new ChatModel();
            model.AllRooms = new RoomsModel();
            model.ConnectedRooms = new RoomsModel();

            model.Users = MvcApplication.Db.GetUsers();
            model.ConnectedRooms.Rooms = MvcApplication.Db.GetRooms((string)Session["nick"]);
            model.AllRooms.Rooms = MvcApplication.Db.GetRooms();

            MvcApplication.Pub.Publish(Resources.Internals.RoomsEventChannel,
                JsonConvert.SerializeObject(MvcApplication.Db.GetRooms()));

            return View(model);
        }

        /// <summary>
        /// Log the user out
        /// </summary>
        /// <returns>Redirects the user to the <see cref="IndexController"/></returns>
        public ActionResult Disconnect() {
            List<string> rooms = MvcApplication.Db.GetRooms((string)Session["nick"]);

            MvcApplication.Db.DelUser(rooms, (string)Session["nick"]);
            MvcApplication.Db.DelUserFromGlobalList((string)Session["nick"]);

            Session.Abandon();

            return RedirectToAction("Index", "Index");
        }

    }
}
