using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Recaptcha;
using webchat.Models;
using System.Web.Security;
using webchat.Helpers;
using webchat.Database;
using Newtonsoft.Json;

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
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [RecaptchaControlMvc.CaptchaValidator]
        public ActionResult Index(IndexModel indexModel, bool captchaValid, string captchaErrorMessage) {
            if(!captchaValid) {
                ModelState.AddModelError("captcha", Resources.Strings.CaptchaError);
            }
            else if(ModelState.IsValid) {
                if(1 == indexModel.Rooms.Count && "" == indexModel.Rooms[0].Trim()) {
                    indexModel.Rooms[0] = Resources.Strings.DefaultRoom;
                }

                indexModel.Rooms.AddUser(indexModel.Nick);
                Publisher.Publish(Resources.Strings.UsersEventChannel,
                    JsonConvert.SerializeObject(indexModel.Rooms.GetUsers()));
                
                Session["nick"] = indexModel.Nick;
                
                return RedirectToAction("Index", "Chat");
            }

            return View(indexModel);
        }
    }
}
