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

            lock(Locker.locker) {
                model.Users = MvcApplication.db.GetUsers();
                model.Rooms = MvcApplication.db.GetRooms((string)Session["nick"]);
            }

            return View(model);
        }

        public ActionResult Disconnect() {
            lock(Locker.locker) {
                List<string> rooms = MvcApplication.db.GetRooms((string)Session["nick"]);

                MvcApplication.db.DelUser(rooms, (string)Session["nick"]);
                MvcApplication.db.DelUserFromGlobalList((string)Session["nick"]);
                MvcApplication.pub.Publish(Resources.Strings.UsersEventChannel,
                    JsonConvert.SerializeObject(MvcApplication.db.GetUsers()));
            }

            Session.Abandon();

            return RedirectToAction("Index", "Index");
        }

    }
}
