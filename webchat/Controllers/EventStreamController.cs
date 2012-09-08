using ServiceStack.Redis;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web.Http;
using webchat.Filters;

namespace webchat.Controllers
{
    public class EventStreamController : ApiController {
        [AuthenticationFilter]
        public HttpResponseMessage Get(HttpRequestMessage request){

            HttpResponseMessage response = request.CreateResponse();
            response.Content = new PushStreamContent(OnStreamAvailable, "text/event-stream");

            return response;
        }
        
        //TODO: refactor these
        public static void OnStreamAvailable(Stream stream, HttpContent content, TransportContext context) {
            StreamWriter streamwriter = new StreamWriter(stream);

            using(var redis = new RedisClient()) {
                using(var sub = redis.CreateSubscription()) {
                    sub.OnMessage = (channel, msg) => {
                        streamwriter.WriteLine("data: " + msg + " on: " + channel + "\n");
                        streamwriter.Flush();

                        Debug.WriteLine("data: " + msg + " on: " + channel);
                    };

                    sub.OnSubscribe = channel => {
                        Debug.WriteLine(string.Format("Subscribed to '{0}'", channel));
                    };

                    sub.OnUnSubscribe = channel => {
                        Debug.WriteLine(string.Format("UnSubscribed from '{0}'", channel));
                    };

                    sub.SubscribeToChannels("webchat");
                }
            }
        }
    }
}
