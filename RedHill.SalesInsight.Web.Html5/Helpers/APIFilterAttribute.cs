using RedHill.SalesInsight.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;

namespace RedHill.SalesInsight.Web.Html5.Helpers
{
    public class APIFilterAttribute : ActionFilterAttribute
    {
        public CompanySetting CompanySettings { get; set; }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            this.CompanySettings = SIDAL.GetCompanySettings();

            if (this.CompanySettings != null)
            {
                filterContext.Controller.ViewBag.AUJSAPIEnabled = this.CompanySettings.EnableAPI;
            }
        }
    }
}