using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using webchat.Database;
using webchat.Filters;
using webchat.Helpers;
using webchat.Models;

namespace webchat.Controllers
{
    /// <summary>
    /// Handle the incoming messages
    /// </summary>
    /// <remarks>In order to send messages the user must be authenticated 
    /// <seealso cref="AuthenticationFilterAttribute"/></remarks>
    [AuthenticationFilter]
    public class MessageController : Controller
    {
        /// <summary>
        /// Publish the message to every chat user on the corresponding channel
        /// </summary>
        /// <param name="m">The model which contains the intended message and channel
        /// <seealso cref="MessageModel"/></param>
        /// <returns>Returns a <seealso cref="HttpStatusCode"/> which represents 
        /// whether the opperation was successful or not</returns>
        [HttpPost]
        [ValidateInput(false)]
        public HttpStatusCode Post(MessageModel m) {
            if(!ModelState.IsValid){
                return HttpStatusCode.BadRequest;
            }

            Dictionary<string, string> data = new Dictionary<string, string>(){
                {"nick", (string)Session["nick"]},
                {"message", m.Message},
                {"room", m.Room}
            };

            MvcApplication.Pub.Publish(Resources.Internals.MessagesEventChannel, JsonConvert.SerializeObject(data));
            MvcApplication.Logger.Log(string.Format("{0} ({1}): {2}", Session["nick"], m.Room, m.Message), "INFO");

            return HttpStatusCode.OK;
        }
    }
}
