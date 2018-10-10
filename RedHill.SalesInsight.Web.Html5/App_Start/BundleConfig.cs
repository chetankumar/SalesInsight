using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;

namespace RedHill.SalesInsight.Web.Html5.App_Start
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/js").Include("~/Scripts/passwordreset.js",
                "~/Scripts/extends.js",
                "~/Scripts/master_select.js",
                "~/Scripts/js/behaviour/general.js",
                "~/Scripts/user_support.js"));

            bundles.Add(new ScriptBundle("~/js/si_chat").Include("~/Scripts/si_chat.js"));

            bundles.Add(new ScriptBundle("~/js/vendors").Include("~/Scripts/js/jquery.upload/js/jquery.fileupload.js",
                "~/Scripts/js/jquery.fileupload-ui.js"));

            bundles.Add(new ScriptBundle("~/js/project_loc").Include("~/Scripts/project_loc.js"));

            bundles.Add(new StyleBundle("~/theme").Include("~/Content/style.css"));

            bundles.Add(new StyleBundle("~/login").Include("~/Content/login.css"));
        }
    }
}