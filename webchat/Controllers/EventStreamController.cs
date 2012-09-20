using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using webchat.Database;
using webchat.Filters;
using webchat.Helpers;

namespace webchat.Controllers
{
    public class EventStreamController : ApiController {        
        [AuthenticationFilter]
        public HttpResponseMessage Get(HttpRequestMessage request){
            HttpResponseMessage response = request.CreateResponse();
            
            response.Content = new PushStreamContent(OnStreamAvailable, "text/event-stream");

            response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue();
            response.Headers.CacheControl.NoCache = true;

            return response;
        }
        
        private void OnStreamAvailable(Stream stream, HttpContent content, TransportContext context) {
            StreamWriter streamWriter = new StreamWriter(stream);
            //Publisher.Clients.Enqueue(streamWriter); //TODO: remove me
            MvcApplication.pub.Clients.Enqueue(streamWriter);
        }
    }
}
