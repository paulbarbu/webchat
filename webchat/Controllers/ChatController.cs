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
            ChatModel model = new ChatModel();

            model.Users = MvcApplication.Db.GetUsers();
            model.Rooms = MvcApplication.Db.GetRooms((string)Session["nick"]);

            return View(model);
        }

        public ActionResult Disconnect() {
            List<string> rooms = MvcApplication.Db.GetRooms((string)Session["nick"]);

            MvcApplication.Db.DelUser(rooms, (string)Session["nick"]);
            MvcApplication.Db.DelUserFromGlobalList((string)Session["nick"]);

            Session.Abandon();

            return RedirectToAction("Index", "Index");
        }

    }
}
