using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace webchat {
    /// <summary>
    /// 
    /// </summary>
    public static class WebApiConfig {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        public static void Register(HttpConfiguration config) {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
