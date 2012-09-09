using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Recaptcha;
using webchat.Models;
using System.Diagnostics;
using ServiceStack.Redis;
using System.Web.Security;

namespace webchat.Controllers
{
    public class IndexController : Controller
    {
        public ActionResult Index(){
            if(Session["nick"] != null) {
                return RedirectToAction("Index", "Chat");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[RecaptchaControlMvc.CaptchaValidator]
        public ActionResult Index(IndexModel indexModel, /*bool captchaValid,*/ string captchaErrorMessage) {
            /*if(!captchaValid) {
                ModelState.AddModelError("captcha", Resources.Strings.CaptchaError);
            }
            else */if(ModelState.IsValid) {
                try {
                    indexModel.Rooms.AddUser(indexModel.Nick);
                    indexModel.Rooms.Notify();
                }
                catch(RedisException) {
                    ModelState.AddModelError("error", Resources.Strings.DatabaseError);

                    return View(indexModel);
                }
                
                Session["nick"] = indexModel.Nick;
                
                return RedirectToAction("Index", "Chat");
            }

            return View(indexModel);
        }
    }
}
