using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webchat.Filters;
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
            catch(RedisException) {
                //TODO: log
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
            catch(RedisException) {
                //TODO: log the failure but get the user out
            }

            Session.Abandon();

            return RedirectToAction("Index", "Index");
        }

    }
}
