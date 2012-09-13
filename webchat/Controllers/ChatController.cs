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
    [AuthenticationFilter]
    public class ChatController : Controller
    {
        public ActionResult Index() {
            Rooms rooms = new Rooms((string)Session["nick"]);

            return View(rooms);
        }

        public ActionResult Disconnect() {
            Rooms rooms = new Rooms((string)Session["nick"]);

            rooms.DelUser((string)Session["nick"]);
            Publisher.Publish(Resources.Strings.UsersEventChannel, JsonConvert.SerializeObject(rooms.GetUsers()));

            Session.Abandon();

            return RedirectToAction("Index", "Index");
        }

    }
}
