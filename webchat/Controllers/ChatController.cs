using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using webchat.Models;

namespace webchat.Controllers
{
    public class ChatController : Controller
    {
        public ActionResult Index()
        {
            if(null == Session["nick"]) {
                return RedirectToAction("Index", "Index");
            }

            Rooms rooms = null;

            try {
                //throw new RedisException("bye");
                rooms = new Rooms((string)Session["nick"]);
            }
            catch(RedisException) {
                ModelState.AddModelError("error", Resources.Strings.DatabaseError);
            }

            return View(rooms);
        }

    }
}
