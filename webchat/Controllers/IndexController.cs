using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Recaptcha;
using webchat.Models;
using System.Diagnostics;
using ServiceStack.Redis;
using ServiceStack.Redis.Generic;

namespace webchat.Controllers
{
    public class IndexController : Controller
    {
        //
        // GET: /Index/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [RecaptchaControlMvc.CaptchaValidator]
        public ActionResult Index(IndexModel indexModel, bool captchaValid, string captchaErrorMessage) {
            if(Session["nick"] != null) {
                RedirectToAction("Chat", "Chat"); // TODO: implement this action/controller
            }

            if(!captchaValid) {
                ModelState.AddModelError("captcha", "Invalid captcha words, please try again!");
            }
            else if(ModelState.IsValid) {
                Session["nick"] = indexModel.Nick;

                indexModel.insertNick();
                indexModel.notify();
                //TODO: add nick to DB, Session["nick"]
                //TODO: add nick to the rooms he joined
                //TODO: publish the changes and go to chat
                //TODO: regenerate session
            }

            return View(indexModel);
        }
    }
}
