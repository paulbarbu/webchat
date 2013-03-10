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
using webchat.Communication;
using webchat.Database;
using webchat.Logging;
using webchat.Ping;

namespace webchat {
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    /// <summary>
    /// 
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication {

        /// <summary>
        /// A singleton of a <see cref="Communication.IPublisher&lt;T&gt;"/> to use throughout the application
        /// </summary>
        public static IPublisher<ConcurrentQueue<StreamWriter>> Pub = new Publisher();

        /// <summary>
        /// A singleton of an <see cref="Logging.ILogger"/> to use throughout the application
        /// </summary>
        public static ILogger Logger = new Logger();

        /// <summary>
        /// A singleton of an <see cref="Database.IDatabase"/> to use throughout the application
        /// </summary>
        public static IDatabase Db = new Db(Pub, Logger);

        /// <summary>
        /// 
        /// </summary>
        protected void Application_Start() {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            Trace.Listeners.Add(new TextWriterTraceListener(Server.MapPath(Resources.Internals.LogFile)));
            Trace.AutoFlush = true;

            Pinger p = new Pinger(); // start pinging users
        }
    }
}