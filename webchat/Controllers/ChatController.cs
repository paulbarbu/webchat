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
        //
        // GET: /Chat/

        public ActionResult Index()
        {
            if(null == Session["nick"]) {
                return RedirectToAction("Index", "Index");
            }

            ChatModel chatModel = new ChatModel();
            try {
                chatModel.Rooms = new Rooms((string)Session["nick"]);
            }
            catch(RedisException) {
                //TODO:
            }

            return View(chatModel);
        }

    }
}
