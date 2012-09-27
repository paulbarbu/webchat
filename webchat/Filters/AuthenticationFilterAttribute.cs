using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace webchat.Filters {
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple=false, Inherited=true)]
    public class AuthenticationFilterAttribute : AuthorizeAttribute, IAuthorizationFilter {

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