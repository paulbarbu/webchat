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
    /// <summary>
    /// The controller that handles the authentication page
    /// </summary>
    /// <remarks><seealso cref="Filters.AuthenticationFilterAttribute"/></remarks>
    public class IndexController : Controller
    {
        /// <summary>
        /// Display the authentication page or the chat page, depending if the user has authenticated or not
        /// </summary>
        /// <returns>Redirects the user to the chat if he's already authenticated or displays the login page</returns>
        public ActionResult Index(){
            if(Session["nick"] != null) {
                return RedirectToAction("Index", "Chat");
            }

            return View();
        }

        /// <summary>
        /// Process the authentication of the user
        /// </summary>
        /// <param name="indexModel">The model that has the user's data binded to</param>
        /// <param name="captchaValid">Whether the captcha is valid or not</param>
        /// <param name="captchaErrorMessage">If the user failed to provide a valid captcha this holds the reason</param>
        /// <returns>Returns an <see cref="Models.IndexModel"/> populated with data 
        /// if the user submitted invalid data or redirects the user to the chat if he logged in</returns>
        /// <remarks>This also handles the CSRF token which prevents an attacker to submit the form remotely.
        /// If the user doesn't provide some rooms to join, he's automatically connected to a default one</remarks>
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

                MvcApplication.Db.AddUser(indexModel.Rooms, indexModel.Nick);
                
                Session["nick"] = indexModel.Nick;
                
                return RedirectToAction("Index", "Chat");
            }

            return View(indexModel);
        }
    }
}
