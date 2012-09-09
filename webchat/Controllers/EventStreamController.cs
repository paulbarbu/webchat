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
using System.Web;
using System.Web.Http;
using webchat.Filters;

namespace webchat.Controllers
{
    public class EventStreamController : ApiController {
        private delegate void Writer(string data);
        
        [AuthenticationFilter]
        public HttpResponseMessage Get(HttpRequestMessage request){
            HttpResponseMessage response = request.CreateResponse();
            
            response.Content = new PushStreamContent(OnStreamAvailable, "text/event-stream");
            response.Headers.CacheControl = new System.Net.Http.Headers.CacheControlHeaderValue();
            response.Headers.CacheControl.NoCache = true;

            return response;
        }
        
        private static void OnStreamAvailable(Stream stream, HttpContent content, TransportContext context) {
            string eventPattern = "event: {0}\ndata: {1}\n\n";
            StreamWriter streamWriter = new StreamWriter(stream);

            Dictionary<string, Writer> write = new Dictionary<string, Writer> {
                { Resources.Strings.MessagesEventChannel , (data) => {
                    streamWriter.Write(eventPattern, "message", data);
                } },
                { Resources.Strings.UsersEventChannel , (data) => {
                    streamWriter.Write(eventPattern, "users", data);
                } },
                { Resources.Strings.PingEventChannel , (data) => {
                    streamWriter.Write(eventPattern, "ping", data);
                } },
                { "error" , (data) => {
                    streamWriter.Write(eventPattern, "error", data);
                } },
            };
            
            try {
                using(var redis = new RedisClient()) {
                    using(var sub = redis.CreateSubscription()) {
                        sub.OnMessage = (channel, data) => {
                            write[channel](data);
                            streamWriter.Flush();
                        };

                        sub.OnUnSubscribe = channel => {
                            streamWriter.Flush();
                        };
                        
                        sub.SubscribeToChannels(Resources.Strings.MessagesEventChannel,
                            Resources.Strings.UsersEventChannel,
                            Resources.Strings.PingEventChannel
                        );
                    }
                }
            }
            catch(RedisException) {
                //TODO: log
                write["error"](Resources.Strings.DatabaseError);
            }
            catch(Exception) {
                //TODO: log
                write["error"](Resources.Strings.UnexpectedError);
            }

            //If we reach this point we caught an exception
            streamWriter.Flush();
            streamWriter.Close();
        }
    }
}
