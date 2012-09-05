using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Recaptcha;
using webchat.Models;

using System.Diagnostics;

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
            Debug.WriteLine("nick: " + indexModel.Nick);
            /*int i = 0;
            foreach(var room in indexModel.Rooms) {
                Debug.WriteLine(string.Format("{0}: {1}", i, room));
                i++;
            }*/

            if(!captchaValid) {
                ModelState.AddModelError("captcha", "Invalid captcha words, please try again!");
            }
            else if(ModelState.IsValid) {
                //this is good
            }

            return View(indexModel);
        }
    }
}
