using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace webchat.Filters {
    /// <summary>
    /// Checks whether the user is logged in or not
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class AuthenticationFilterAttribute : AuthorizeAttribute, IAuthorizationFilter {

        /// <summary>
        /// Does the actual authorization
        /// </summary>
        /// <param name="authorizationContext">Object that holds HTTP and Session data</param>
        /// <remarks>If the user is not logged in he's automatically redirected to the Index</remarks>
        public override void OnAuthorization(AuthorizationContext authorizationContext) {
            if(null == authorizationContext.HttpContext.Session["nick"]) {
                authorizationContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(new {
                        action = "Index",
                        controller = "Index",
                    })
                );
            }
        }
    }
}