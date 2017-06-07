using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace Accounting.AccountAuthorize
{
    public class AccountAutherize : AuthorizeAttribute
    {

        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {


            
            //base.HandleUnauthorizedRequest(filterContext);
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (!filterContext.HttpContext.User.IsInRole(Roles))
                {
                    filterContext.Result = new RedirectToRouteResult(
                                   new RouteValueDictionary
                                   {
                                       { "action", "login" },
                                       { "controller", "staff" },
                                       {"ReturnUrl",filterContext.HttpContext.Request.Url },
                                       { "area",""}
                                   });
                }
                //var requestContext = HttpContext.Current.Request.RequestContext;
                //string loginurl = string.Empty;
                //if (filterContext.HttpContext.Request.IsSecureConnection)
                //{
                //    loginurl = string.Format("https://{0}/staff/login?ReturnUrl={1}", filterContext.HttpContext.Request.Url.Authority, Uri.EscapeDataString(filterContext.HttpContext.Request.Url.ToString()));
                //}
                //else
                //{
                //    loginurl = string.Format("http://{0}/staff/login?ReturnUrl={1}", filterContext.HttpContext.Request.Url.Authority, Uri.EscapeDataString(filterContext.HttpContext.Request.Url.ToString()));

                //}
                ////string abspath = HttpContext.Current.Request.Url.AbsoluteUri;
                //filterContext.HttpContext.Response.Redirect(loginurl);
                //filterContext.Result = new RedirectToRouteResult(
                //                   new RouteValueDictionary
                //                   {
                //                       { "action", "login" },
                //                       { "controller", "staff" },
                //                       {"ReturnUrl",filterContext.HttpContext.Request.Url },
                //                       { "area",""}
                //                   });
            }
            else
            {
                filterContext.Result = new RedirectToRouteResult(
                                   new RouteValueDictionary
                                   {
                                       { "action", "login" },
                                       { "controller", "staff" },
                                       {"ReturnUrl",filterContext.HttpContext.Request.Url },
                                       { "area",""}
                                   });
            }
            
        }


    }
}