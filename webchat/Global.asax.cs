using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using webchat.Database;
using webchat.Ping;

namespace webchat {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication {

        public static IPublisher<ConcurrentQueue<StreamWriter>> pub = new Publisher();

        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Trace.Listeners.Add(new TextWriterTraceListener(Server.MapPath(Resources.Strings.LogFile)));
            Trace.AutoFlush = true;

            Pinger p = new Pinger(); // start pinging users
        }
    }
}