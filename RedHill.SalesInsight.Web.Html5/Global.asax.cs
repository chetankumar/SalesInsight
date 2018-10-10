using RedHill.SalesInsight.DAL;
using RedHill.SalesInsight.DAL.DataTypes;
using RedHill.SalesInsight.Web.Html5.App_Start;
using RedHill.SalesInsight.Web.Html5.Models.Validators;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace RedHill.SalesInsight.Web.Html5
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_BeginRequest()
        {
            CultureInfo info = new CultureInfo(System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
            info.DateTimeFormat.ShortDatePattern = "MM/dd/yyyy";
            System.Threading.Thread.CurrentThread.CurrentCulture = info;
        }

        protected void Application_Start()
        {
            RouteTable.Routes.MapHubs();
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //ModelBinders.Binders.Add(typeof(decimal),new CustomBinder());
            //ModelBinders.Binders.Add(typeof(decimal?), new CustomBinder());
            //ModelBinders.Binders.Add(typeof(int), new CustomBinder());
            //ModelBinders.Binders.Add(typeof(int?), new CustomBinder());
            BootstrapSupport.BootstrapBundleConfig.RegisterBundles(System.Web.Optimization.BundleTable.Bundles);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
            // look if any security information exists for this request
            if (HttpContext.Current.User != null)
            {
                // see if this user is authenticated, any authenticated cookie (ticket) exists for this user
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    // see if the authentication is done using FormsAuthentication
                    if (HttpContext.Current.User.Identity is FormsIdentity)
                    {
                        // Get the roles stored for this request from the ticket
                        // get the identity of the user
                        FormsIdentity identity = (FormsIdentity)HttpContext.Current.User.Identity;
                        //Get the form authentication ticket of the user
                        FormsAuthenticationTicket ticket = identity.Ticket;
                        //Get the roles stored as UserData into ticket

                        Guid userId = (Guid)Membership.GetUser(User.Identity.Name).ProviderUserKey;
                        SIUser user = SIDAL.GetUser(userId.ToString());
                        string[] roles = new string[]{user.Role};

                        FormsIdentity formsIdentity = new FormsIdentity(ticket);

                        SIRoleAccess access = SIDAL.GetAccessRules(user.Role);
                        
                        //Create general prrincipal and assign it to current request

                        HttpContext.Current.User = new System.Security.Principal.GenericPrincipal(identity, roles);
                    }
                }
            }
        }

        //protected void Application_EndRequest()
        //{
        //    if (Request.IsSecureConnection == true && HttpContext.Current.Request.Url.Scheme == "https")
        //    {
        //        Request.Cookies["ASP.NET_SessionID"].Secure = true;
        //        if (Request.Cookies.Count > 0)
        //        {
        //            foreach (string s in Request.Cookies.AllKeys)
        //            {
        //                Request.Cookies[s].Secure = true;
        //            }
        //        }

        //        Response.Cookies["ASP.NET_SessionID"].Secure = true;
        //        if (Response.Cookies.Count > 0)
        //        {
        //            foreach (string s in Response.Cookies.AllKeys)
        //            {
        //                Response.Cookies[s].Secure = true;
        //            }
        //        }
        //    }

        //    Response.Headers.Remove("Server");
        //    Response.Headers.Remove("X-AspNet-Version");
        //}
    }
}