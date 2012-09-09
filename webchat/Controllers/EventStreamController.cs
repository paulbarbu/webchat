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
        
        //TODOL enum for the event type
        //TODO: refactor these
        private static void OnStreamAvailable(Stream stream, HttpContent content, TransportContext context) {
            StreamWriter writer = new StreamWriter(stream);
            string eventPattern = "event: {0}\ndata: {1}\n\n";
            
            try {
                using(var redis = new RedisClient()) {
                    using(var sub = redis.CreateSubscription()) {
                        sub.OnMessage = (channel, msg) => {
                            writer.Write(eventPattern, "message", "data: " + msg + " on: " + channel);
                            writer.Flush();
                        };

                        sub.OnUnSubscribe = channel => {
                            writer.Flush();
                        };

                        string[] a = new string[] {                            
                            "webchat.room.*",
                            "webchat.users*",
                            "webchat.ping*"
                        };

                        sub.SubscribeToChannels("webchat.ping", "webchat.users");
                    }
                }
            }
            catch(RedisException) {
                //TODO: error, log
                writer.Write(eventPattern, "error", "data: " + Resources.Strings.DatabaseError);
            }
            catch(Exception) {
                //TODO: error, log
                writer.Write(eventPattern, "error", "data: " + Resources.Strings.UnexpectedError);
            }
            
            //If we reach this point we caught an exception
            writer.Flush();
            writer.Close();
        }
    }
}
