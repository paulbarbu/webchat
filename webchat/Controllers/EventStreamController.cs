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
    /// <summary>
    /// Handle the Eventstream connections
    /// </summary>
    /// <remarks>The user cannot access this if he's not authenticated <seealso cref="AuthenticationFilterAttribute"/></remarks>
    public class EventStreamController : ApiController {     
        /// <summary>
        /// Prepare the response headers for the EventStream
        /// </summary>
        /// <param name="request">The request from which the response will be generated</param>
        /// <returns>Returns <see cref="HttpResponseMessage"/> whose content will be changed by <see cref="MvcApplication.Pub"/></returns>
        [AuthenticationFilter]
        public HttpResponseMessage Get(HttpRequestMessage request){
            HttpResponseMessage response = request.CreateResponse();
            
            response.Content = new PushStreamContent(OnStreamAvailable, "text/event-stream");

            response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue();
            response.Headers.CacheControl.NoCache = true;

            return response;
        }
        /// <summary>
        /// Registers the connection as a client to <see cref="MvcApplication.Pub"/>
        /// </summary>
        /// <param name="stream">The stream from which the StreamWriter will be created</param>
        /// <param name="content"></param>
        /// <param name="context"></param>
        /// <remarks>This is a callback for <see cref="PushStreamContent"/></remarks>
        private void OnStreamAvailable(Stream stream, HttpContent content, TransportContext context) {
            StreamWriter streamWriter = new StreamWriter(stream);
            MvcApplication.Pub.Clients.Enqueue(streamWriter);
        }
    }
}
