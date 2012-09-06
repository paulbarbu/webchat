using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

            return View();
        }

    }
}
