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
            return View(Db.GetRooms((string)Session["nick"]));
        }

        public ActionResult Disconnect() {
            List<string> rooms = Db.GetRooms((string)Session["nick"]);

            Db.DelUser(rooms, (string)Session["nick"]);
            Publisher.Publish(Resources.Strings.UsersEventChannel, JsonConvert.SerializeObject(Db.GetUsers()));

            Session.Abandon();

            return RedirectToAction("Index", "Index");
        }

    }
}
