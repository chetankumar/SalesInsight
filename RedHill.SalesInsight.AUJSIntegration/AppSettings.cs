using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace RedHill.SalesInsight.AUJSIntegration
{
    public class AppSettings
    {
        #region API Settings

        public static string APIBaseURL = ConfigurationManager.AppSettings["API_BASE_URL"];
        public static string ClientId = ConfigurationManager.AppSettings["CLIENT_ID"];
        public static string ClientKey = ConfigurationManager.AppSettings["CLIENT_KEY"];

        #endregion

        public static string APISource = ConfigurationManager.AppSettings["API_SOURCE"];
    }
}
