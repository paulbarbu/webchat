using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webchat.Filters;
using webchat.Helpers;
using webchat.Models;

namespace webchat.Controllers
{
    [AuthenticationFilter]
    public class ChatController : Controller
    {
        public ActionResult Index() {
            Rooms rooms = null;

            try {
                rooms = new Rooms((string)Session["nick"]);
            }
            catch(RedisException e) {
                Logger.Log(Resources.Strings.DatabaseError, "ERROR");
                Logger.Log(e.ToString(), "ERROR");

                ModelState.AddModelError("error", Resources.Strings.DatabaseError);
            }

            return View(rooms);
        }

        public ActionResult Disconnect() {
            Rooms rooms = null;
            
            try {
                rooms = new Rooms((string)Session["nick"]);
                rooms.DelUser((string)Session["nick"]);
                rooms.Notify();
            }
            catch(RedisException e) {
                Logger.Log(Resources.Strings.DatabaseError, "ERROR");
                Logger.Log(e.ToString(), "ERROR");
            }

            Session.Abandon();

            return RedirectToAction("Index", "Index");
        }

    }
}
