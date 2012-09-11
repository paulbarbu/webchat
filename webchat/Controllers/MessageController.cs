using Newtonsoft.Json;
using ServiceStack.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using webchat.Filters;
using webchat.Models;

namespace webchat.Controllers
{
    [AuthenticationFilter]
    public class MessageController : Controller
    {
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
            
            try {                
                Publish(JsonConvert.SerializeObject(data));
                //TODO: log
            }
            catch(RedisException){
                //TODO: log
                return HttpStatusCode.InternalServerError;
            }

            return HttpStatusCode.OK;
        }

        private void Publish(string msg){
            using(var r = new RedisClient()) {
                r.PublishMessage(Resources.Strings.MessagesEventChannel, msg);
            }
        }
    }
}
